﻿using AccountingOffice.Application.Events;
using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Application.UseCases.AccountReceiv.Commands;
using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Enums;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<AccountReceivableCommandHandler> _logger;

    public AccountReceivableCommandHandler(
        IAccountReceivableRepository accountReceivableRepository, 
        IAccountReceivableQuery accountReceivableQuery, 
        IPersonQuery personQuery,
        IApplicationBus applicationBus,
        ILogger<AccountReceivableCommandHandler> logger)
    {
        _accountReceivableRepository = accountReceivableRepository ?? throw new ArgumentNullException(nameof(accountReceivableRepository));
        _accountReceivableQuery = accountReceivableQuery ?? throw new ArgumentNullException(nameof(accountReceivableQuery));
        _personQuery = personQuery ?? throw new ArgumentNullException(nameof(personQuery));
        _applicationBus = applicationBus ?? throw new ArgumentNullException(nameof(applicationBus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<Result<Guid>> Handle(CreateAccountReceivableCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var accountId = Guid.NewGuid();
        
        _logger.LogInformation(
            "Iniciando criação de conta a receber. AccountId: {AccountId}, TenantId: {TenantId}, CustomerId: {CustomerId}, Amount: {Amount}, DueDate: {DueDate}",
            accountId, command.TenantId, command.CustomerId, command.Amount, command.DueDate);

        Person<Guid>? person = await _personQuery.GetByIdAsync(command.TenantId, command.CustomerId);
        if (person is null)
        {
            _logger.LogWarning(
                "Cliente não encontrado ao criar conta a receber. AccountId: {AccountId}, TenantId: {TenantId}, CustomerId: {CustomerId}",
                accountId, command.TenantId, command.CustomerId);
            return Result<Guid>.Failure("Cliente não localizado.");
        }

        DomainResult<AccountReceivable> domainResult = AccountReceivable.Create(accountId,
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
            _logger.LogWarning(
                "Falha na criação de conta a receber. AccountId: {AccountId}, TenantId: {TenantId}, Error: {Error}",
                accountId, command.TenantId, domainResult.Error);
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
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Conta a receber criada com sucesso. AccountId: {AccountId}, TenantId: {TenantId}, Amount: {Amount}, DurationMs: {DurationMs}",
            domainResult.Value.Id, command.TenantId, command.Amount, stopwatch.ElapsedMilliseconds);
        
        return Result<Guid>.Success(domainResult.Value.Id);
    }

    public async Task<Result<bool>> Handle(UpdateAccountReceivableCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando atualização de conta a receber. AccountId: {AccountId}, TenantId: {TenantId}, HasDescription: {HasDescription}, HasDueDate: {HasDueDate}, HasPayMethod: {HasPayMethod}",
            command.Id, command.TenantId, command.HasDescription, command.HasDueDate, command.HasPayMethod);

        AccountReceivable? accountReceivable = await _accountReceivableQuery.GetByIdAsync(command.TenantId, command.Id);
        if (accountReceivable is null)
        {
            _logger.LogWarning(
                "Conta a receber não encontrada para atualização. AccountId: {AccountId}, TenantId: {TenantId}",
                command.Id, command.TenantId);
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
        {
            _logger.LogWarning(
                "Falha na atualização de conta a receber. AccountId: {AccountId}, TenantId: {TenantId}, Errors: {Errors}",
                command.Id, command.TenantId, string.Join("; ", errors));
            return Result<bool>.Failure(string.Join("; ", errors));
        }

        await _accountReceivableRepository.UpdateAsync(accountReceivable);

        // Publicar evento de conta atualizada
        var @event = new AccountUpdatedEvent(
            accountReceivable.Id,
            accountReceivable.TenantId,
            accountReceivable.Ammount,
            DateTime.UtcNow
        );
        
        await _applicationBus.PublishEvent(@event, cancellationToken);
        stopwatch.Stop();

        _logger.LogInformation(
            "Conta a receber atualizada com sucesso. AccountId: {AccountId}, TenantId: {TenantId}, DurationMs: {DurationMs}",
            command.Id, command.TenantId, stopwatch.ElapsedMilliseconds);

        return Result<bool>.Success(true);

    }

    public async Task<Result<bool>> Handle(DeleteAccountReceivableCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando exclusão de conta a receber. AccountId: {AccountId}, TenantId: {TenantId}",
            command.Id, command.TenantId);

        AccountReceivable? accountReceivable = await _accountReceivableQuery.GetByIdAsync(command.TenantId, command.Id);
        if (accountReceivable is null)
        {
            _logger.LogWarning(
                "Conta a receber não encontrada para exclusão. AccountId: {AccountId}, TenantId: {TenantId}",
                command.Id, command.TenantId);
            return Result<bool>.Failure("Conta a receber não localizada.");
        }

        var amount = accountReceivable.Ammount;
        await _accountReceivableRepository.DeleteAsync(accountReceivable.Id);

        // Publicar evento de conta excluída (podemos usar o evento de atualização com valores zerados)
        var @event = new AccountUpdatedEvent(
            accountReceivable.Id,
            accountReceivable.TenantId,
            0, // Valor zerado para indicar exclusão
            DateTime.UtcNow
        );
        
        await _applicationBus.PublishEvent(@event, cancellationToken);
        stopwatch.Stop();

        _logger.LogInformation(
            "Conta a receber excluída com sucesso. AccountId: {AccountId}, TenantId: {TenantId}, Amount: {Amount}, DurationMs: {DurationMs}",
            command.Id, command.TenantId, amount, stopwatch.ElapsedMilliseconds);

        return Result<bool>.Success(true);

    }
}
