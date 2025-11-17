using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Application.UseCases.AccountReceiv.Commands;
using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Enums;

namespace AccountingOffice.Application.UseCases.AccountReceiv.CommandHandler;

public class AccountReceivableCommandHandler :
    ICommandHandler<CreateAccountReceivableCommand, Result<Guid>>,
    ICommandHandler<UpdateAccountReceivableCommand, Result<bool>>,
    ICommandHandler<DeleteAccountReceivableCommand, Result<bool>>
{
    private readonly IAccountReceivableRepository _accountReceivableRepository;
    private readonly IAccountReceivableQuery _accountReceivableQuery;
    private readonly IPersonQuery _personQuery;

    public AccountReceivableCommandHandler(IAccountReceivableRepository accountReceivableRepository, IAccountReceivableQuery accountReceivableQuery, IPersonQuery personQuery)
    {
        _accountReceivableRepository = accountReceivableRepository ?? throw new ArgumentNullException(nameof(accountReceivableRepository));
        _accountReceivableQuery = accountReceivableQuery ?? throw new ArgumentNullException(nameof(accountReceivableQuery));
        _personQuery = personQuery ?? throw new ArgumentNullException(nameof(personQuery));
    }
    public async Task<Result<Guid>> Handle(CreateAccountReceivableCommand command, CancellationToken cancellationToken)
    {
        Person<Guid>? person = await _personQuery.GetByIdAsync(command.TenantId, command.CustomerId);
        if (person is null)
        {
            return Result<Guid>.Failure("Cliente não localizado.");
        }

        DomainResult<AccountReceivable> domainResult = AccountReceivable.Create(Guid.NewGuid(),
                                                                command.TenantId,
                                                                command.Description,
                                                                command.Amount,
                                                                command.DueDate,
                                                                command.IssueDate,
                                                                AccountStatus.Pending,
                                                                person,
                                                                (PaymentMethod)command.PayMethod,
                                                                command.InvoiceNumber);

        if (domainResult.IsFailure)
        {
            return Result<Guid>.Failure(domainResult.Error);
        }

        await _accountReceivableRepository.CreateAsync(domainResult.Value);
        return Result<Guid>.Success(domainResult.Value.Id);
    }

    public async Task<Result<bool>> Handle(UpdateAccountReceivableCommand command, CancellationToken cancellationToken)
    {
        AccountReceivable? accountReceivable = await _accountReceivableQuery.GetByIdAsync(command.TenantId, command.Id);
        if (accountReceivable is null)
        {
            return Result<bool>.Failure("Conta a receber não localizada."); 
        }

        List<string> errors = new();

        if (command.HasDescription)
        { 
            DomainResult result = accountReceivable.ChangeDescription(command.Description);
            if (result.IsFailure)
                errors.Add(result.Error);
        }
        if (command.HasDueDate)
        {
            DomainResult result = accountReceivable.ChangeDueDate(command.DueDate);
            if (result.IsFailure)
                errors.Add(result.Error);
        }
        if (command.HasPayMethod)
        {
            DomainResult result = accountReceivable.ChangePaymentMethod((PaymentMethod)command.PayMethod);
            if (result.IsFailure)
                errors.Add(result.Error);
        }

        if (errors.Any())
            return Result<bool>.Failure(string.Join("; ", errors));

        await _accountReceivableRepository.UpdateAsync(accountReceivable);

        return Result<bool>.Success(true);

    }

    public async Task<Result<bool>> Handle(DeleteAccountReceivableCommand command, CancellationToken cancellationToken)
    {
        AccountReceivable? accountReceivable = await _accountReceivableQuery.GetByIdAsync(command.TenantId, command.Id);
        if (accountReceivable is null)
        {
            return Result<bool>.Failure("Conta a receber não localizada.");
        }

        await _accountReceivableRepository.DeleteAsync(accountReceivable.Id);
        return Result<bool>.Success(true);

    }
}
