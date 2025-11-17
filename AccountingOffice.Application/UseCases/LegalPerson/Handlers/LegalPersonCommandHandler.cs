using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Application.UseCases.Legal.Commands;
using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Common;

namespace AccountingOffice.Application.UseCases.Legal.Handlers;

public class LegalPersonCommandHandler :
    ICommandHandler<CreateLegalPersonCommand, Result<Guid>>,
    ICommandHandler<UpdateLegalPersonCommand, Result<bool>>,
    ICommandHandler<DeleteLegalPersonCommand, Result<bool>>
{
    private readonly ILegalPersonRepository _legalPersonRepository;
    private readonly ILegalPersonQuery _legalPersonQuery;

    public LegalPersonCommandHandler(ILegalPersonRepository legalPersonRepository, ILegalPersonQuery legalPersonQuery)
    {
        _legalPersonRepository = legalPersonRepository ?? throw new ArgumentNullException(nameof(legalPersonRepository));
        _legalPersonQuery = legalPersonQuery ?? throw new ArgumentNullException(nameof(legalPersonQuery));
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
        return Result<Guid>.Success(domainResult.Value.Id);
    }

    public async Task<Result<bool>> Handle(UpdateLegalPersonCommand command, CancellationToken cancellationToken)
    {
        LegalPerson? legalPerson = await _legalPersonQuery.GetByIdAsync(command.Id, command.TenantId);

        if (legalPerson is null)
        {
            return Result<bool>.Failure("Pessoa jurídica não encontrada.");
        }

        if (legalPerson.Phone != command.PhoneNumber) legalPerson.ChangePhone(command.PhoneNumber);

        if (legalPerson.Email != command.Email) legalPerson.ChangeEmail(command.Email);

        if (legalPerson.Name != command.Name) legalPerson.ChangeName(command.Name);

        if (legalPerson.LegalName != command.LegalName)
        {
            DomainResult changeResult = legalPerson.ChangeLegalName(command.LegalName);
            if (!changeResult.IsSuccess)
                return Result<bool>.Failure(changeResult.Error);
        }

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
