﻿﻿using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Application.UseCases.Legal.Commands;
using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Common;
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

    public LegalPersonCommandHandler(
        ILegalPersonRepository legalPersonRepository, 
        ILegalPersonQuery legalPersonQuery,
        IApplicationBus applicationBus)
    {
        _legalPersonRepository = legalPersonRepository ?? throw new ArgumentNullException(nameof(legalPersonRepository));
        _legalPersonQuery = legalPersonQuery ?? throw new ArgumentNullException(nameof(legalPersonQuery));
        _applicationBus = applicationBus ?? throw new ArgumentNullException(nameof(applicationBus));
    }

    public async Task<Result<Guid>> Handle(CreateLegalPersonCommand command, CancellationToken cancellationToken)
    {
        DomainResult<LegalPerson> domainResult = LegalPerson.Create(Guid.NewGuid(),
                                                                    command.TenantId,
                                                                    command.Name,
                                                                    command.Document,
                                                                    Domain.Core.Enums.PersonType.Company,
                                                                    command.Email,
                                                                    command.PhoneNumber,
                                                                    command.LegalName);

        if (!domainResult.IsSuccess)
            return Result<Guid>.Failure(domainResult.Error);

        await _legalPersonRepository.CreateAsync(domainResult.Value);
        
        // Publicar evento genérico para pessoas jurídicas
        // Como não há evento específico para pessoa jurídica, usamos um evento genérico
        var @event = new object(); // Placeholder para evento futuro
        
        return Result<Guid>.Success(domainResult.Value.Id);
    }

    public async Task<Result<bool>> Handle(UpdateLegalPersonCommand command, CancellationToken cancellationToken)
    {
        LegalPerson? legalPerson = await _legalPersonQuery.GetByIdAsync(command.Id, command.TenantId);

        if (legalPerson is null)
            return Result<bool>.Failure("Pessoa jurídica não encontrada.");

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
            return Result<bool>.Failure(string.Join("|", errors.Select(x => x)));

        await _legalPersonRepository.UpdateAsync(legalPerson);

        return Result<bool>.Success(true);
    }


    public async Task<Result<bool>> Handle(DeleteLegalPersonCommand command, CancellationToken cancellationToken)
    {
        LegalPerson? legalPerson = await _legalPersonQuery.GetByIdAsync(command.Id, command.TenantId);

        if (legalPerson is null)
        {
            return Result<bool>.Failure("Pessoa jurídica não encontrada.");
        }

        await _legalPersonRepository.DeleteAsync(legalPerson.Id);
        return Result<bool>.Success(true);
    }
}
