﻿﻿using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Application.UseCases.Cia.Commands;
using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Common;
using Microsoft.Extensions.Logging;

namespace AccountingOffice.Application.UseCases.Cia.CommandHandler;

public class CompanyCommandHandler :
    ICommandHandler<CreateCompanyCommand, Result<Guid>>,
    ICommandHandler<UpdateCompanyCommand, Result<bool>>,
    ICommandHandler<DeleteCompanyCommand, Result<bool>>,
    ICommandHandler<ToggleCompanyActiveStatusCommand, Result<bool>>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyQuery _companyQuery;
    private readonly ILogger<CompanyCommandHandler> _logger;

    public CompanyCommandHandler(
        ICompanyRepository companyRepository, 
        ICompanyQuery companyQuery,
        ILogger<CompanyCommandHandler> logger)
    {
        _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        _companyQuery = companyQuery ?? throw new ArgumentNullException(nameof(companyQuery));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<Result<Guid>> Handle(CreateCompanyCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var companyId = Guid.NewGuid();
        
        _logger.LogInformation(
            "Iniciando criação de companhia. CompanyId: {CompanyId}, Document: {Document}, Name: {Name}",
            companyId, command.Document, command.Name);

        DomainResult<Company> cia = Company.Create(companyId,
                                                   command.Name,
                                                   command.Document,
                                                   command.Email,
                                                   command.Phone);

        if (cia.IsFailure)
        {
            _logger.LogWarning(
                "Falha na criação de companhia. CompanyId: {CompanyId}, Document: {Document}, Error: {Error}",
                companyId, command.Document, cia.Error);
            return Result<Guid>.Failure(cia.Error);
        }

        await _companyRepository.CreateAsync(cia.Value);
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Companhia criada com sucesso. CompanyId: {CompanyId}, Document: {Document}, Name: {Name}, DurationMs: {DurationMs}",
            cia.Value.Id, command.Document, command.Name, stopwatch.ElapsedMilliseconds);
        
        var @event = new object();
        
        return Result<Guid>.Success(cia.Value.Id);
    }

    public async Task<Result<bool>> Handle(UpdateCompanyCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando atualização de companhia. CompanyId: {CompanyId}, HasName: {HasName}, HasEmail: {HasEmail}, HasPhone: {HasPhone}",
            command.Id, command.HasName, command.HasEmail, command.HasPhone);

        Company? cia = await _companyQuery.GetByIdAsync(command.Id);
        if (cia is null)
        {
            _logger.LogWarning("Companhia não encontrada para atualização. CompanyId: {CompanyId}", command.Id);
            return Result<bool>.Failure("Companhia não localizada.");
        }
        List<string> errors = new();

        if (command.HasName)
        {
            DomainResult updateNameResult = cia.ChangeName(command.Name);
            if (updateNameResult.IsFailure)
                errors.Add(updateNameResult.Error);
        }

        if (command.HasEmail)
        {
            DomainResult updateEmailResult = cia.ChangeEmail(command.Email);
            if (updateEmailResult.IsFailure)
                errors.Add(updateEmailResult.Error);
        }

        if (command.HasPhone)
        {
            DomainResult updatePhoneResult = cia.ChangePhone(command.Phone);
            if (updatePhoneResult.IsFailure)
                errors.Add(updatePhoneResult.Error);
        }

        if (errors.Any())
        {
            _logger.LogWarning(
                "Falha na atualização de companhia. CompanyId: {CompanyId}, Errors: {Errors}",
                command.Id, string.Join("|", errors));
            return Result<bool>.Failure(string.Join("|", errors));
        }

        await _companyRepository.UpdateAsync(cia);
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Companhia atualizada com sucesso. CompanyId: {CompanyId}, DurationMs: {DurationMs}",
            command.Id, stopwatch.ElapsedMilliseconds);
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> Handle(DeleteCompanyCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation("Iniciando exclusão de companhia. CompanyId: {CompanyId}", command.Id);

        Company? cia = await _companyQuery.GetByIdAsync(command.Id);
        if (cia is null)
        {
            _logger.LogWarning("Companhia não encontrada para exclusão. CompanyId: {CompanyId}", command.Id);
            return Result<bool>.Failure("Companhia não localizada.");
        }
        
        await _companyRepository.DeleteAsync(cia.Id);
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Companhia excluída com sucesso. CompanyId: {CompanyId}, Document: {Document}, DurationMs: {DurationMs}",
            cia.Id, cia.Document, stopwatch.ElapsedMilliseconds);
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> Handle(ToggleCompanyActiveStatusCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando alteração de status de companhia. CompanyId: {CompanyId}, Active: {Active}",
            command.Id, command.Active);

        Company? cia = await _companyQuery.GetByIdAsync(command.Id);
        if (cia is null)
        {
            _logger.LogWarning("Companhia não encontrada para alteração de status. CompanyId: {CompanyId}", command.Id);
            return Result<bool>.Failure("Companhia não localizada.");
        }

        var previousStatus = cia.Active;
        if (command.Active)
            cia.Activate();
        else
            cia.Deactivate();
        
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Status de companhia alterado com sucesso. CompanyId: {CompanyId}, PreviousStatus: {PreviousStatus}, NewStatus: {NewStatus}, DurationMs: {DurationMs}",
            command.Id, previousStatus, cia.Active, stopwatch.ElapsedMilliseconds);
        
        return Result<bool>.Success(true);
    }
}
