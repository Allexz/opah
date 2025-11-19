﻿﻿﻿﻿using AccountingOffice.Application.Events;
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
    private readonly IApplicationBus _applicationBus;
    
    public AccountPayableCommandHandler(
        IAccountPayableQuery accountPayableQuery, 
        IAccountPayableRepository accountPayableRepository, 
        IPersonQuery personQuery,
        IApplicationBus applicationBus)
    {
        _accountPayableQuery = accountPayableQuery ?? throw new ArgumentException(nameof(accountPayableQuery));
        _accountPayableRepository = accountPayableRepository ?? throw new ArgumentException(nameof(accountPayableRepository));
        _personQuery = personQuery ?? throw new ArgumentException(nameof(personQuery));
        _applicationBus = applicationBus ?? throw new ArgumentException(nameof(applicationBus));
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
        
         var @event = new AccountPayableCreatedEvent(
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

        // Publicar evento de conta atualizada
        var @event = new AccountUpdatedEvent(
            accountPayable.Id,
            accountPayable.TenantId,
            accountPayable.Ammount,
            DateTime.UtcNow
        );
        
        await _applicationBus.PublishEvent(@event, cancellationToken);

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

        // Publicar evento de conta excluída (podemos usar o evento de atualização com valores zerados)
        var @event = new AccountUpdatedEvent(
            accountPayable.Id,
            accountPayable.TenantId,
            0, // Valor zerado para indicar exclusão
            DateTime.UtcNow
        );
        
        await _applicationBus.PublishEvent(@event, cancellationToken);

        return Result<bool>.Success(true);
    }
}
