﻿﻿﻿﻿using AccountingOffice.Application.Events;
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
    private readonly IApplicationBus _applicationBus;

    public AccountReceivableCommandHandler(
        IAccountReceivableRepository accountReceivableRepository, 
        IAccountReceivableQuery accountReceivableQuery, 
        IPersonQuery personQuery,
        IApplicationBus applicationBus)
    {
        _accountReceivableRepository = accountReceivableRepository ?? throw new ArgumentNullException(nameof(accountReceivableRepository));
        _accountReceivableQuery = accountReceivableQuery ?? throw new ArgumentNullException(nameof(accountReceivableQuery));
        _personQuery = personQuery ?? throw new ArgumentNullException(nameof(personQuery));
        _applicationBus = applicationBus ?? throw new ArgumentNullException(nameof(applicationBus));
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
        
         var @event = new AccountReceivableCreatedEvent(
            domainResult.Value.Id,
            domainResult.Value.TenantId,
            domainResult.Value.Ammount,
            domainResult.Value.IssueDate,
            domainResult.Value.DueDate,
            domainResult.Value.Description
        );
        
         await _applicationBus.PublishEvent(@event, cancellationToken);
        
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

        // Publicar evento de conta atualizada
        var @event = new AccountUpdatedEvent(
            accountReceivable.Id,
            accountReceivable.TenantId,
            accountReceivable.Ammount,
            DateTime.UtcNow
        );
        
        await _applicationBus.PublishEvent(@event, cancellationToken);

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

        // Publicar evento de conta excluída (podemos usar o evento de atualização com valores zerados)
        var @event = new AccountUpdatedEvent(
            accountReceivable.Id,
            accountReceivable.TenantId,
            0, // Valor zerado para indicar exclusão
            DateTime.UtcNow
        );
        
        await _applicationBus.PublishEvent(@event, cancellationToken);

        return Result<bool>.Success(true);

    }
}
