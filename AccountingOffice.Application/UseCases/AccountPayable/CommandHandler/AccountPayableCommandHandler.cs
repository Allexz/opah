﻿using AccountingOffice.Application.Events;
using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Application.UseCases.AccountPay.Commands;
using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Enums;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<AccountPayableCommandHandler> _logger;
    
    public AccountPayableCommandHandler(
        IAccountPayableQuery accountPayableQuery, 
        IAccountPayableRepository accountPayableRepository, 
        IPersonQuery personQuery,
        IApplicationBus applicationBus,
        ILogger<AccountPayableCommandHandler> logger)
    {
        _accountPayableQuery = accountPayableQuery ?? throw new ArgumentException(nameof(accountPayableQuery));
        _accountPayableRepository = accountPayableRepository ?? throw new ArgumentException(nameof(accountPayableRepository));
        _personQuery = personQuery ?? throw new ArgumentException(nameof(personQuery));
        _applicationBus = applicationBus ?? throw new ArgumentException(nameof(applicationBus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<Result<Guid>> Handle(CreateAccountPayableCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var accountId = Guid.NewGuid();
        
        _logger.LogInformation(
            "Iniciando criação de conta a pagar. AccountId: {AccountId}, TenantId: {TenantId}, SupplierId: {SupplierId}, Amount: {Amount}, DueDate: {DueDate}",
            accountId, command.TenantId, command.SupplierId, command.Ammount, command.DueDate);

        Person<Guid>? person = await _personQuery.GetByIdAsync(command.SupplierId, command.TenantId);
        if (person is null)
        {
            _logger.LogWarning(
                "Fornecedor não encontrado ao criar conta a pagar. AccountId: {AccountId}, TenantId: {TenantId}, SupplierId: {SupplierId}",
                accountId, command.TenantId, command.SupplierId);
            return Result<Guid>.Failure("Fornecedor não encontrado.");
        }

        DomainResult<AccountPayable> domainResult = AccountPayable.Create(accountId,
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
        {
            _logger.LogWarning(
                "Falha na criação de conta a pagar. AccountId: {AccountId}, TenantId: {TenantId}, Error: {Error}",
                accountId, command.TenantId, domainResult.Error);
            return Result<Guid>.Failure(domainResult.Error);
        }

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
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Conta a pagar criada com sucesso. AccountId: {AccountId}, TenantId: {TenantId}, Amount: {Amount}, DurationMs: {DurationMs}",
            domainResult.Value.Id, command.TenantId, command.Ammount, stopwatch.ElapsedMilliseconds);
        
        return Result<Guid>.Success(domainResult.Value.Id);
    }

    public async Task<Result<bool>> Handle(UpdateAccountPayableCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando atualização de conta a pagar. AccountId: {AccountId}, TenantId: {TenantId}, HasPaymentDate: {HasPaymentDate}, HasStatus: {HasStatus}, HasDescription: {HasDescription}, HasPayMethod: {HasPayMethod}",
            command.Id, command.TenantId, command.HasPaymentDate, command.HasStatus, command.HasDescription, command.HasPayMethod);

        AccountPayable? accountPayable = await _accountPayableQuery.GetByIdAsync(command.Id, command.TenantId);
        if (accountPayable is null)
        {
            _logger.LogWarning(
                "Conta a pagar não encontrada para atualização. AccountId: {AccountId}, TenantId: {TenantId}",
                command.Id, command.TenantId);
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
        stopwatch.Stop();

        _logger.LogInformation(
            "Conta a pagar atualizada com sucesso. AccountId: {AccountId}, TenantId: {TenantId}, DurationMs: {DurationMs}",
            command.Id, command.TenantId, stopwatch.ElapsedMilliseconds);

        return Result<bool>.Success(true);

    }

    public async Task<Result<bool>> Handle(DeleteAccountPayableCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando exclusão de conta a pagar. AccountId: {AccountId}, TenantId: {TenantId}",
            command.Id, command.TenantId);

        AccountPayable? accountPayable = await _accountPayableQuery.GetByIdAsync(command.Id, command.TenantId);
        if (accountPayable is null)
        {
            _logger.LogWarning(
                "Conta a pagar não encontrada para exclusão. AccountId: {AccountId}, TenantId: {TenantId}",
                command.Id, command.TenantId);
            return Result<bool>.Failure("Conta a pagar não encontrada.");
        }

        var amount = accountPayable.Ammount;
        await _accountPayableRepository.DeleteAsync(accountPayable.Id);

        // Publicar evento de conta excluída (podemos usar o evento de atualização com valores zerados)
        var @event = new AccountUpdatedEvent(
            accountPayable.Id,
            accountPayable.TenantId,
            0, // Valor zerado para indicar exclusão
            DateTime.UtcNow
        );
        
        await _applicationBus.PublishEvent(@event, cancellationToken);
        stopwatch.Stop();

        _logger.LogInformation(
            "Conta a pagar excluída com sucesso. AccountId: {AccountId}, TenantId: {TenantId}, Amount: {Amount}, DurationMs: {DurationMs}",
            command.Id, command.TenantId, amount, stopwatch.ElapsedMilliseconds);

        return Result<bool>.Success(true);
    }
}
