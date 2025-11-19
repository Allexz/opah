﻿﻿using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Application.UseCases.Individual.Commands;
using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Enums;
using Microsoft.Extensions.Logging;

namespace AccountingOffice.Application.UseCases.Individual.CommandHandler;

public class IndividualPersonCommandHandler :
    ICommandHandler<CreateIndividualPersonCommand, Result<Guid>>,
    ICommandHandler<UpdateIndividualPersonCommand, Result<bool>>,
    ICommandHandler<DeleteIndividualPersonCommand, Result<bool>>
{
    private readonly IIndividualPersonRepository _individualPersonRepository;
    private readonly IIndividualPersonQuery _individualPersonQuery;
    private readonly IApplicationBus _applicationBus;
    private readonly ILogger<IndividualPersonCommandHandler> _logger;

    public IndividualPersonCommandHandler(
        IIndividualPersonRepository individualPersonRepository, 
        IIndividualPersonQuery individualPersonQuery,
        IApplicationBus applicationBus,
        ILogger<IndividualPersonCommandHandler> logger)
    {
        _individualPersonRepository = individualPersonRepository ?? throw new ArgumentNullException(nameof(individualPersonRepository));
        _individualPersonQuery = individualPersonQuery ?? throw new ArgumentNullException(nameof(individualPersonQuery));
        _applicationBus = applicationBus ?? throw new ArgumentNullException(nameof(applicationBus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<Result<Guid>> Handle(CreateIndividualPersonCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var personId = Guid.NewGuid();
        
        _logger.LogInformation(
            "Iniciando criação de pessoa física. PersonId: {PersonId}, TenantId: {TenantId}, Document: {Document}, Name: {Name}",
            personId, command.TenantId, command.Document, command.Name);

        DomainResult<IndividualPerson> domainResult = IndividualPerson.Create(personId,
                                                                        command.TenantId,
                                                                        command.Name,
                                                                        command.Document,
                                                                        Domain.Core.Enums.PersonType.Individual,
                                                                        command.Email,
                                                                        command.PhoneNumber,
                                                                        (MaritalStatus)command.MaritalStatus);

        if (!domainResult.IsSuccess)
        {
            _logger.LogWarning(
                "Falha na criação de pessoa física. PersonId: {PersonId}, TenantId: {TenantId}, Document: {Document}, Error: {Error}",
                personId, command.TenantId, command.Document, domainResult.Error);
            return Result<Guid>.Failure(domainResult.Error);
        }

        await _individualPersonRepository.CreateAsync(domainResult.Value);
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Pessoa física criada com sucesso. PersonId: {PersonId}, TenantId: {TenantId}, Document: {Document}, DurationMs: {DurationMs}",
            domainResult.Value.Id, command.TenantId, command.Document, stopwatch.ElapsedMilliseconds);
        
        // Publicar evento genérico para pessoas físicas
        // Como não há evento específico para pessoa física, usamos um evento genérico
        var @event = new object(); // Placeholder para evento futuro
        
        return Result<Guid>.Success(domainResult.Value.Id);
    }

    public async Task<Result<bool>> Handle(UpdateIndividualPersonCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando atualização de pessoa física. PersonId: {PersonId}, TenantId: {TenantId}, HasPhoneNumber: {HasPhoneNumber}, HasEmail: {HasEmail}, HasName: {HasName}, HasMaritalStatus: {HasMaritalStatus}",
            command.Id, command.TenantId, command.HasPhoneNumber, command.HasEmail, command.HasName, command.HasMaritalStatus);

        IndividualPerson? individualPerson = await _individualPersonQuery.GetByIdAsync(command.Id, command.TenantId);

        if (individualPerson is null)
        {
            _logger.LogWarning(
                "Pessoa física não encontrada para atualização. PersonId: {PersonId}, TenantId: {TenantId}",
                command.Id, command.TenantId);
            return Result<bool>.Failure("Pessoa física não encontrada.");
        }

        if (command.HasPhoneNumber) individualPerson.ChangePhone(command.PhoneNumber);
        if (command.HasEmail) individualPerson.ChangeEmail(command.Email);
        if (command.HasName) individualPerson.ChangeName(command.Name);
        if (command.HasMaritalStatus)individualPerson.ChangeMaritalStatus((MaritalStatus)command.MaritalStatus);

        await _individualPersonRepository.UpdateAsync(individualPerson);
        stopwatch.Stop();

        _logger.LogInformation(
            "Pessoa física atualizada com sucesso. PersonId: {PersonId}, TenantId: {TenantId}, DurationMs: {DurationMs}",
            command.Id, command.TenantId, stopwatch.ElapsedMilliseconds);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> Handle(DeleteIndividualPersonCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando exclusão de pessoa física. PersonId: {PersonId}, TenantId: {TenantId}",
            command.Id, command.TenantId);

        IndividualPerson? individualPerson = await _individualPersonQuery.GetByIdAsync(command.Id, command.TenantId);

        if (individualPerson is null)
        {
            _logger.LogWarning(
                "Pessoa física não encontrada para exclusão. PersonId: {PersonId}, TenantId: {TenantId}",
                command.Id, command.TenantId);
            return Result<bool>.Failure("Pessoa física não encontrada.");
        }

        var document = individualPerson.Document;
        await _individualPersonRepository.DeleteAsync(individualPerson.Id);
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Pessoa física excluída com sucesso. PersonId: {PersonId}, TenantId: {TenantId}, Document: {Document}, DurationMs: {DurationMs}",
            command.Id, command.TenantId, document, stopwatch.ElapsedMilliseconds);
        
        return Result<bool>.Success(true);
    }
}
