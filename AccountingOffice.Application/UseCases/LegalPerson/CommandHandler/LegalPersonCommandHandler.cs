﻿﻿using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Application.UseCases.Legal.Commands;
using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Common;
using Microsoft.Extensions.Logging;
using System.Text;

namespace AccountingOffice.Application.UseCases.Legal.CommandHandler;

public class LegalPersonCommandHandler :
    ICommandHandler<CreateLegalPersonCommand, Result<Guid>>,
    ICommandHandler<UpdateLegalPersonCommand, Result<bool>>,
    ICommandHandler<DeleteLegalPersonCommand, Result<bool>>
{
    private readonly ILegalPersonRepository _legalPersonRepository;
    private readonly ILegalPersonQuery _legalPersonQuery;
    private readonly IApplicationBus _applicationBus;
    private readonly ILogger<LegalPersonCommandHandler> _logger;

    public LegalPersonCommandHandler(
        ILegalPersonRepository legalPersonRepository, 
        ILegalPersonQuery legalPersonQuery,
        IApplicationBus applicationBus,
        ILogger<LegalPersonCommandHandler> logger)
    {
        _legalPersonRepository = legalPersonRepository ?? throw new ArgumentNullException(nameof(legalPersonRepository));
        _legalPersonQuery = legalPersonQuery ?? throw new ArgumentNullException(nameof(legalPersonQuery));
        _applicationBus = applicationBus ?? throw new ArgumentNullException(nameof(applicationBus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<Guid>> Handle(CreateLegalPersonCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var personId = Guid.NewGuid();
        
        _logger.LogInformation(
            "Iniciando criação de pessoa jurídica. PersonId: {PersonId}, TenantId: {TenantId}, Document: {Document}, Name: {Name}, LegalName: {LegalName}",
            personId, command.TenantId, command.Document, command.Name, command.LegalName);

        DomainResult<LegalPerson> domainResult = LegalPerson.Create(personId,
                                                                    command.TenantId,
                                                                    command.Name,
                                                                    command.Document,
                                                                    Domain.Core.Enums.PersonType.Company,
                                                                    command.Email,
                                                                    command.PhoneNumber,
                                                                    command.LegalName);

        if (!domainResult.IsSuccess)
        {
            _logger.LogWarning(
                "Falha na criação de pessoa jurídica. PersonId: {PersonId}, TenantId: {TenantId}, Document: {Document}, Error: {Error}",
                personId, command.TenantId, command.Document, domainResult.Error);
            return Result<Guid>.Failure(domainResult.Error);
        }

        await _legalPersonRepository.CreateAsync(domainResult.Value);
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Pessoa jurídica criada com sucesso. PersonId: {PersonId}, TenantId: {TenantId}, Document: {Document}, DurationMs: {DurationMs}",
            domainResult.Value.Id, command.TenantId, command.Document, stopwatch.ElapsedMilliseconds);
        
        // Publicar evento genérico para pessoas jurídicas
        // Como não há evento específico para pessoa jurídica, usamos um evento genérico
        var @event = new object(); // Placeholder para evento futuro
        
        return Result<Guid>.Success(domainResult.Value.Id);
    }

    public async Task<Result<bool>> Handle(UpdateLegalPersonCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando atualização de pessoa jurídica. PersonId: {PersonId}, TenantId: {TenantId}, HasPhoneNumber: {HasPhoneNumber}, HasEmail: {HasEmail}, HasName: {HasName}, HasLegalName: {HasLegalName}",
            command.Id, command.TenantId, command.HasPhoneNumber, command.HasEmail, command.Hasname, command.HasLegalName);

        LegalPerson? legalPerson = await _legalPersonQuery.GetByIdAsync(command.Id, command.TenantId);

        if (legalPerson is null)
        {
            _logger.LogWarning(
                "Pessoa jurídica não encontrada para atualização. PersonId: {PersonId}, TenantId: {TenantId}",
                command.Id, command.TenantId);
            return Result<bool>.Failure("Pessoa jurídica não encontrada.");
        }

        var errors = new List<string>();

        if (command.HasPhoneNumber)
        {
            var result = legalPerson.ChangePhone(command.PhoneNumber);
            if (!result.IsSuccess) errors.Add(result.Error);
        }

        if (command.HasEmail)
        {
            var result = legalPerson.ChangeEmail(command.Email);
            if (!result.IsSuccess) errors.Add(result.Error);
        }

        if (command.Hasname)
        {
            var result = legalPerson.ChangeName(command.Name);
            if (!result.IsSuccess) errors.Add(result.Error);
        }

        if (command.HasLegalName)
        {
            var result = legalPerson.ChangeLegalName(command.LegalName);
            if (!result.IsSuccess) errors.Add(result.Error);
        }

        if (errors.Any())
        {
            _logger.LogWarning(
                "Falha na atualização de pessoa jurídica. PersonId: {PersonId}, TenantId: {TenantId}, Errors: {Errors}",
                command.Id, command.TenantId, string.Join("|", errors));
            return Result<bool>.Failure(string.Join("|", errors.Select(x => x)));
        }

        await _legalPersonRepository.UpdateAsync(legalPerson);
        stopwatch.Stop();

        _logger.LogInformation(
            "Pessoa jurídica atualizada com sucesso. PersonId: {PersonId}, TenantId: {TenantId}, DurationMs: {DurationMs}",
            command.Id, command.TenantId, stopwatch.ElapsedMilliseconds);

        return Result<bool>.Success(true);
    }


    public async Task<Result<bool>> Handle(DeleteLegalPersonCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando exclusão de pessoa jurídica. PersonId: {PersonId}, TenantId: {TenantId}",
            command.Id, command.TenantId);

        LegalPerson? legalPerson = await _legalPersonQuery.GetByIdAsync(command.Id, command.TenantId);

        if (legalPerson is null)
        {
            _logger.LogWarning(
                "Pessoa jurídica não encontrada para exclusão. PersonId: {PersonId}, TenantId: {TenantId}",
                command.Id, command.TenantId);
            return Result<bool>.Failure("Pessoa jurídica não encontrada.");
        }

        var document = legalPerson.Document;
        await _legalPersonRepository.DeleteAsync(legalPerson.Id);
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Pessoa jurídica excluída com sucesso. PersonId: {PersonId}, TenantId: {TenantId}, Document: {Document}, DurationMs: {DurationMs}",
            command.Id, command.TenantId, document, stopwatch.ElapsedMilliseconds);
        
        return Result<bool>.Success(true);
    }
}
