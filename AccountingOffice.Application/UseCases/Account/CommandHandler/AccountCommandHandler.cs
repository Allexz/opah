using AccountingOffice.Application.Events;
using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Application.UseCases.Installm.Commands;
using AccountingOffice.Application.UseCases.Installments.Commands;
using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Enums;
using AccountingOffice.Domain.Core.ValueObjects;

namespace AccountingOffice.Application.UseCases.Account.CommandHandler;

public class AccountCommandHandler :
    ICommandHandler<AddInstallmentCommand, Result<bool>>,
    ICommandHandler<DeleteInstallmentCommand, Result<bool>>,
    ICommandHandler<ChangeInstallmentStatusCommand, Result<bool>>,
    ICommandHandler<PayInstallmentCommand, Result<bool>>
{
    private readonly IInstalmentRepository _instalmentRepository;
    private readonly IAccountPayableQuery _accountPayableQuery;
    private readonly IAccountReceivableQuery _accountReceivableQuery;
    private readonly IApplicationBus _applicationBus;

    public AccountCommandHandler(
        IInstalmentRepository instalmentRepository,
        IAccountPayableQuery accountPayableQuery,
        IAccountReceivableQuery accountReceivableQuery,
        IApplicationBus applicationBus)
    {
        _instalmentRepository = instalmentRepository ?? throw new ArgumentException(nameof(instalmentRepository));
        _accountPayableQuery = accountPayableQuery ?? throw new ArgumentException(nameof(accountPayableQuery));
        _accountReceivableQuery = accountReceivableQuery ?? throw new ArgumentException(nameof(accountReceivableQuery));
        _applicationBus = applicationBus ?? throw new ArgumentException(nameof(applicationBus));
    }

    public async Task<Result<bool>> Handle(AddInstallmentCommand command, CancellationToken cancellationToken)
    {
        Account<Guid>? account = await GetAccountByEntryType(command.AccountId, command.TenantId, (EntryType)command.entryType, cancellationToken);
        if (account is null)
        {
            return Result<bool>.Failure("Conta não encontrada.");
        }

        DomainResult<Installment> domainResult = Installment.Create(command.InstallmentNumber,
                                                                     command.Amount,
                                                                     command.DueDate,
                                                                     AccountStatus.Pending,
                                                                     (EntryType)command.entryType);
        if (domainResult.IsFailure)
            return Result<bool>.Failure(domainResult.Error);

        DomainResult addResult = account.AddInstallment(domainResult.Value);
        if (addResult.IsFailure)
            return Result<bool>.Failure(addResult.Error);

        await _instalmentRepository.CreateAsync(domainResult.Value);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> Handle(DeleteInstallmentCommand command, CancellationToken cancellationToken)
    {
        Account<Guid>? account = await GetAccountByEntryType(command.AccountId, command.TenantId, EntryType.Debit, cancellationToken);
        if (account is null)
        {
            account = await GetAccountByEntryType(command.AccountId, command.TenantId, EntryType.Credit, cancellationToken);
            if (account is null)
            {
                return Result<bool>.Failure("Conta não encontrada.");
            }
        }

        Installment? installment = account.Installments.FirstOrDefault(i => i.InstallmentNumber == command.InstallmentNumber);
        if (installment is null)
        {
            return Result<bool>.Failure("Parcela não encontrada.");
        }

        if (installment.IsPaid)
        {
            return Result<bool>.Failure("Não é possível excluir uma parcela que já foi paga.");
        }

        await _instalmentRepository.DeleteAsync(command.AccountId, command.TenantId, command.InstallmentNumber);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> Handle(ChangeInstallmentStatusCommand command, CancellationToken cancellationToken)
    {
        Account<Guid>? account = await GetAccountByEntryType(command.AccountId, command.TenantId, EntryType.Debit, cancellationToken);
        if (account is null)
        {
            account = await GetAccountByEntryType(command.AccountId, command.TenantId, EntryType.Credit, cancellationToken);
            if (account is null)
            {
                return Result<bool>.Failure("Conta não encontrada.");
            }
        }

        Installment? installment = account.Installments.FirstOrDefault(i => i.InstallmentNumber == command.InstallmentNumber);
        if (installment is null)
        {
            return Result<bool>.Failure("Parcela não encontrada.");
        }

        DomainResult changeResult = installment.ChangeStatus((AccountStatus)command.accountStatus, DateTime.UtcNow);
        if (changeResult.IsFailure)
            return Result<bool>.Failure(changeResult.Error);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> Handle(PayInstallmentCommand command, CancellationToken cancellationToken)
    {
        Account<Guid>? account = await GetAccountByEntryType(command.AccountId, command.TenantId, EntryType.Debit, cancellationToken);
        if (account is null)
        {
            account = await GetAccountByEntryType(command.AccountId, command.TenantId, EntryType.Credit, cancellationToken);
            if (account is null)
            {
                return Result<bool>.Failure("Conta não encontrada.");
            }
        }

        Installment? installment = account.Installments.FirstOrDefault(i => i.InstallmentNumber == command.InstallmentNumber);
        if (installment is null)
        {
            return Result<bool>.Failure("Parcela não encontrada.");
        }

        // Marcar a parcela como paga
        DomainResult payResult = installment.ChangeStatus(AccountStatus.Paid,installment.PaymentDate);
        if (payResult.IsFailure)
            return Result<bool>.Failure(payResult.Error);

        // Publicar evento de parcela paga
        var @event = new InstallmentPaidEvent(
            command.AccountId,
            command.TenantId,
            command.InstallmentNumber,
            command.PaymentAmount,
            command.PaymentDate
        );
        
        await _applicationBus.PublishEvent(@event, cancellationToken);
        
        return Result<bool>.Success(true);
    }

    private async Task<Account<Guid>?> GetAccountByEntryType(Guid accountId, Guid tenantId, EntryType entryType, CancellationToken cancellationToken)
    {
        if (entryType == EntryType.Debit)
        {
            return await _accountPayableQuery.GetByIdAsync(accountId, tenantId, cancellationToken);
        }
        else if (entryType == EntryType.Credit)
        {
            return await _accountReceivableQuery.GetByIdAsync(accountId, tenantId, cancellationToken);
        }

        return null;
    }
}
