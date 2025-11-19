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
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<AccountCommandHandler> _logger;

    public AccountCommandHandler(
        IInstalmentRepository instalmentRepository,
        IAccountPayableQuery accountPayableQuery,
        IAccountReceivableQuery accountReceivableQuery,
        IApplicationBus applicationBus,
        ILogger<AccountCommandHandler> logger)
    {
        _instalmentRepository = instalmentRepository ?? throw new ArgumentException(nameof(instalmentRepository));
        _accountPayableQuery = accountPayableQuery ?? throw new ArgumentException(nameof(accountPayableQuery));
        _accountReceivableQuery = accountReceivableQuery ?? throw new ArgumentException(nameof(accountReceivableQuery));
        _applicationBus = applicationBus ?? throw new ArgumentException(nameof(applicationBus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<bool>> Handle(AddInstallmentCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando adição de parcela. AccountId: {AccountId}, TenantId: {TenantId}, InstallmentNumber: {InstallmentNumber}, Amount: {Amount}, EntryType: {EntryType}",
            command.AccountId, command.TenantId, command.InstallmentNumber, command.Amount, command.entryType);

        Account<Guid>? account = await GetAccountByEntryType(command.AccountId, command.TenantId, (EntryType)command.entryType, cancellationToken);
        if (account is null)
        {
            _logger.LogWarning(
                "Conta não encontrada ao adicionar parcela. AccountId: {AccountId}, TenantId: {TenantId}, EntryType: {EntryType}",
                command.AccountId, command.TenantId, command.entryType);
            return Result<bool>.Failure("Conta não encontrada.");
        }

        DomainResult<Installment> domainResult = Installment.Create(command.InstallmentNumber,
                                                                     command.Amount,
                                                                     command.DueDate,
                                                                     AccountStatus.Pending,
                                                                     (EntryType)command.entryType);
        if (domainResult.IsFailure)
        {
            _logger.LogWarning(
                "Falha na criação de parcela. AccountId: {AccountId}, TenantId: {TenantId}, InstallmentNumber: {InstallmentNumber}, Error: {Error}",
                command.AccountId, command.TenantId, command.InstallmentNumber, domainResult.Error);
            return Result<bool>.Failure(domainResult.Error);
        }

        DomainResult addResult = account.AddInstallment(domainResult.Value);
        if (addResult.IsFailure)
        {
            _logger.LogWarning(
                "Falha ao adicionar parcela à conta. AccountId: {AccountId}, TenantId: {TenantId}, InstallmentNumber: {InstallmentNumber}, Error: {Error}",
                command.AccountId, command.TenantId, command.InstallmentNumber, addResult.Error);
            return Result<bool>.Failure(addResult.Error);
        }

        await _instalmentRepository.CreateAsync(domainResult.Value);
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Parcela adicionada com sucesso. AccountId: {AccountId}, TenantId: {TenantId}, InstallmentNumber: {InstallmentNumber}, Amount: {Amount}, DurationMs: {DurationMs}",
            command.AccountId, command.TenantId, command.InstallmentNumber, command.Amount, stopwatch.ElapsedMilliseconds);
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> Handle(DeleteInstallmentCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando exclusão de parcela. AccountId: {AccountId}, TenantId: {TenantId}, InstallmentNumber: {InstallmentNumber}",
            command.AccountId, command.TenantId, command.InstallmentNumber);

        Account<Guid>? account = await GetAccountByEntryType(command.AccountId, command.TenantId, EntryType.Debit, cancellationToken);
        if (account is null)
        {
            account = await GetAccountByEntryType(command.AccountId, command.TenantId, EntryType.Credit, cancellationToken);
            if (account is null)
            {
                _logger.LogWarning(
                    "Conta não encontrada ao excluir parcela. AccountId: {AccountId}, TenantId: {TenantId}",
                    command.AccountId, command.TenantId);
                return Result<bool>.Failure("Conta não encontrada.");
            }
        }

        Installment? installment = account.Installments.FirstOrDefault(i => i.InstallmentNumber == command.InstallmentNumber);
        if (installment is null)
        {
            _logger.LogWarning(
                "Parcela não encontrada para exclusão. AccountId: {AccountId}, TenantId: {TenantId}, InstallmentNumber: {InstallmentNumber}",
                command.AccountId, command.TenantId, command.InstallmentNumber);
            return Result<bool>.Failure("Parcela não encontrada.");
        }

        if (installment.IsPaid)
        {
            _logger.LogWarning(
                "Tentativa de excluir parcela já paga. AccountId: {AccountId}, TenantId: {TenantId}, InstallmentNumber: {InstallmentNumber}",
                command.AccountId, command.TenantId, command.InstallmentNumber);
            return Result<bool>.Failure("Não é possível excluir uma parcela que já foi paga.");
        }

        var amount = installment.Amount;
        await _instalmentRepository.DeleteAsync(command.AccountId, command.TenantId, command.InstallmentNumber);
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Parcela excluída com sucesso. AccountId: {AccountId}, TenantId: {TenantId}, InstallmentNumber: {InstallmentNumber}, Amount: {Amount}, DurationMs: {DurationMs}",
            command.AccountId, command.TenantId, command.InstallmentNumber, amount, stopwatch.ElapsedMilliseconds);
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> Handle(ChangeInstallmentStatusCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando alteração de status de parcela. AccountId: {AccountId}, TenantId: {TenantId}, InstallmentNumber: {InstallmentNumber}, NewStatus: {NewStatus}",
            command.AccountId, command.TenantId, command.InstallmentNumber, command.accountStatus);

        Account<Guid>? account = await GetAccountByEntryType(command.AccountId, command.TenantId, EntryType.Debit, cancellationToken);
        if (account is null)
        {
            account = await GetAccountByEntryType(command.AccountId, command.TenantId, EntryType.Credit, cancellationToken);
            if (account is null)
            {
                _logger.LogWarning(
                    "Conta não encontrada ao alterar status de parcela. AccountId: {AccountId}, TenantId: {TenantId}",
                    command.AccountId, command.TenantId);
                return Result<bool>.Failure("Conta não encontrada.");
            }
        }

        Installment? installment = account.Installments.FirstOrDefault(i => i.InstallmentNumber == command.InstallmentNumber);
        if (installment is null)
        {
            _logger.LogWarning(
                "Parcela não encontrada para alteração de status. AccountId: {AccountId}, TenantId: {TenantId}, InstallmentNumber: {InstallmentNumber}",
                command.AccountId, command.TenantId, command.InstallmentNumber);
            return Result<bool>.Failure("Parcela não encontrada.");
        }

        var previousStatus = installment.Status;
        DomainResult changeResult = installment.ChangeStatus((AccountStatus)command.accountStatus, DateTime.UtcNow);
        if (changeResult.IsFailure)
        {
            _logger.LogWarning(
                "Falha ao alterar status de parcela. AccountId: {AccountId}, TenantId: {TenantId}, InstallmentNumber: {InstallmentNumber}, Error: {Error}",
                command.AccountId, command.TenantId, command.InstallmentNumber, changeResult.Error);
            return Result<bool>.Failure(changeResult.Error);
        }
        stopwatch.Stop();

        _logger.LogInformation(
            "Status de parcela alterado com sucesso. AccountId: {AccountId}, TenantId: {TenantId}, InstallmentNumber: {InstallmentNumber}, PreviousStatus: {PreviousStatus}, NewStatus: {NewStatus}, DurationMs: {DurationMs}",
            command.AccountId, command.TenantId, command.InstallmentNumber, previousStatus, installment.Status, stopwatch.ElapsedMilliseconds);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> Handle(PayInstallmentCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando pagamento de parcela. AccountId: {AccountId}, TenantId: {TenantId}, InstallmentNumber: {InstallmentNumber}, PaymentAmount: {PaymentAmount}, PaymentDate: {PaymentDate}",
            command.AccountId, command.TenantId, command.InstallmentNumber, command.PaymentAmount, command.PaymentDate);

        Account<Guid>? account = await GetAccountByEntryType(command.AccountId, command.TenantId, EntryType.Debit, cancellationToken);
        if (account is null)
        {
            account = await GetAccountByEntryType(command.AccountId, command.TenantId, EntryType.Credit, cancellationToken);
            if (account is null)
            {
                _logger.LogWarning(
                    "Conta não encontrada ao pagar parcela. AccountId: {AccountId}, TenantId: {TenantId}",
                    command.AccountId, command.TenantId);
                return Result<bool>.Failure("Conta não encontrada.");
            }
        }

        Installment? installment = account.Installments.FirstOrDefault(i => i.InstallmentNumber == command.InstallmentNumber);
        if (installment is null)
        {
            _logger.LogWarning(
                "Parcela não encontrada para pagamento. AccountId: {AccountId}, TenantId: {TenantId}, InstallmentNumber: {InstallmentNumber}",
                command.AccountId, command.TenantId, command.InstallmentNumber);
            return Result<bool>.Failure("Parcela não encontrada.");
        }

        // Marcar a parcela como paga
        DomainResult payResult = installment.ChangeStatus(AccountStatus.Paid,installment.PaymentDate);
        if (payResult.IsFailure)
        {
            _logger.LogWarning(
                "Falha ao pagar parcela. AccountId: {AccountId}, TenantId: {TenantId}, InstallmentNumber: {InstallmentNumber}, Error: {Error}",
                command.AccountId, command.TenantId, command.InstallmentNumber, payResult.Error);
            return Result<bool>.Failure(payResult.Error);
        }

        // Publicar evento de parcela paga
        var @event = new InstallmentPaidEvent(
            command.AccountId,
            command.TenantId,
            command.InstallmentNumber,
            command.PaymentAmount,
            command.PaymentDate
        );
        
        await _applicationBus.PublishEvent(@event, cancellationToken);
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Parcela paga com sucesso. AccountId: {AccountId}, TenantId: {TenantId}, InstallmentNumber: {InstallmentNumber}, PaymentAmount: {PaymentAmount}, DurationMs: {DurationMs}",
            command.AccountId, command.TenantId, command.InstallmentNumber, command.PaymentAmount, stopwatch.ElapsedMilliseconds);
        
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
