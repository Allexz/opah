using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Application.UseCases.AccountPay.Commands;
using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Enums;

namespace AccountingOffice.Application.UseCases.AccountPay.CommandHandler;

public class AccountPayableCommandHandler :
    ICommandHandler<CreateAccountPayableCommand, Result<Guid>>,
    ICommandHandler<UpdateAccountPayableCommand, Result<bool>>,
    ICommandHandler<DeleteAccountPayableCommand, Result<bool>>
{
    private readonly IAccountPayableRepository _accountPayableRepository;
    private readonly IAccountPayableQuery _accountPayableQuery;
    private readonly IPersonQuery _personQuery;
    public AccountPayableCommandHandler(IAccountPayableQuery accountPayableQuery, IAccountPayableRepository accountPayableRepository, IPersonQuery personQuery)
    {
        _accountPayableQuery = accountPayableQuery ?? throw new ArgumentException(nameof(accountPayableQuery));
        _accountPayableRepository = accountPayableRepository ?? throw new ArgumentException(nameof(accountPayableRepository));
        _personQuery = personQuery ?? throw new ArgumentException(nameof(personQuery));
    }
    public async Task<Result<Guid>> Handle(CreateAccountPayableCommand command, CancellationToken cancellationToken)
    {
        Person<Guid>? person = await _personQuery.GetByIdAsync(command.SupplierId, command.TenantId);
        if (person is null)
        {
            return Result<Guid>.Failure("Fornecedor não encontrado.");
        }

        DomainResult<AccountPayable> domainResult = AccountPayable.Create(Guid.NewGuid(),
                                                          command.TenantId,
                                                          command.Description,
                                                          command.Ammount,
                                                          DateTime.UtcNow,
                                                          command.DueDate,
                                                          (AccountStatus) command.Status,
                                                          person,
                                                          (PaymentMethod)command.PayMethod,
                                                          command.PaymentDate);
        if (domainResult.IsFailure)
            return Result<Guid>.Failure(domainResult.Error);

        await _accountPayableRepository.CreateAsync(domainResult.Value);
        return Result<Guid>.Success(domainResult.Value.Id);

    }

    public async Task<Result<bool>> Handle(UpdateAccountPayableCommand command, CancellationToken cancellationToken)
    {
        AccountPayable? accountPayable = await _accountPayableQuery.GetByIdAsync(command.Id, command.TenantId);
        if (accountPayable is null)
        {
            return Result<bool>.Failure("Conta a pagar não encontrada.");
        }

        if (command.HasPaymentDate) accountPayable.ChangeDueDate(command.PaymentDate!.Value);
        if (command.HasStatus) accountPayable.ChangeStatus((AccountStatus) command.Status);
        if (command.HasDescription) accountPayable.ChangeDescription(command.Description);
        if (command.HasPayMethod) accountPayable.ChangePaymentMethod((PaymentMethod) command.PayMethod);
        
        await _accountPayableRepository.UpdateAsync(accountPayable);

        return Result<bool>.Success(true);

    }

    public async Task<Result<bool>> Handle(DeleteAccountPayableCommand command, CancellationToken cancellationToken)
    {
        AccountPayable? accountPayable = await _accountPayableQuery.GetByIdAsync(command.Id, command.TenantId);
        if (accountPayable is null)
        {
            return Result<bool>.Failure("Conta a pagar não encontrada.");
        }

        await _accountPayableRepository.DeleteAsync(accountPayable.Id);

        return Result<bool>.Success(true);
    }
}
