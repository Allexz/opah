using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Application.UseCases.Individual.Commands;
using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Enums;

namespace AccountingOffice.Application.UseCases.Individual.CommandHandler;

public class IndividualPersonCommandHandler :
    ICommandHandler<CreateIndividualPersonCommand, Result<Guid>>,
    ICommandHandler<UpdateIndividualPersonCommand, Result<bool>>,
    ICommandHandler<DeleteIndividualPersonCommand, Result<bool>>
{
    private readonly IIndividualPersonRepository _individualPersonRepository;
    private readonly IIndividualPersonQuery _individualPersonQuery;

    public IndividualPersonCommandHandler(IIndividualPersonRepository individualPersonRepository, IIndividualPersonQuery individualPersonQuery)
    {
        _individualPersonRepository = individualPersonRepository ?? throw new ArgumentNullException(nameof(individualPersonRepository));
        _individualPersonQuery = individualPersonQuery ?? throw new ArgumentNullException(nameof(individualPersonQuery));
    }
    public async Task<Result<Guid>> Handle(CreateIndividualPersonCommand command, CancellationToken cancellationToken)
    {
        DomainResult<IndividualPerson> domainResult = IndividualPerson.Create(Guid.NewGuid(),
                                                                        command.TenantId,
                                                                        command.Name,
                                                                        command.Document,
                                                                        Domain.Core.Enums.PersonType.Individual,
                                                                        command.Email,
                                                                        command.PhoneNumber,
                                                                        (MaritalStatus)command.MaritalStatus);

        if (!domainResult.IsSuccess)
            return Result<Guid>.Failure(domainResult.Error);

        await _individualPersonRepository.CreateAsync(domainResult.Value);
        return Result<Guid>.Success(domainResult.Value.Id);
    }

    public async Task<Result<bool>> Handle(UpdateIndividualPersonCommand command, CancellationToken cancellationToken)
    {
        IndividualPerson? individualPerson = await _individualPersonQuery.GetByIdAsync(command.Id, command.TenantId);

        if (individualPerson is null)
        {
            return Result<bool>.Failure("Pessoa física não encontrada.");
        }

        if (command.HasPhoneNumber) individualPerson.ChangePhone(command.PhoneNumber);
        if (command.HasEmail) individualPerson.ChangeEmail(command.Email);
        if (command.HasName) individualPerson.ChangeName(command.Name);
        if (command.HasMaritalStatus)individualPerson.ChangeMaritalStatus((MaritalStatus)command.MaritalStatus);

        await _individualPersonRepository.UpdateAsync(individualPerson);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> Handle(DeleteIndividualPersonCommand command, CancellationToken cancellationToken)
    {
        IndividualPerson? individualPerson = await _individualPersonQuery.GetByIdAsync(command.Id, command.TenantId);

        if (individualPerson is null)
        {
            return Result<bool>.Failure("Pessoa física não encontrada.");
        }

        await _individualPersonRepository.DeleteAsync(individualPerson.Id);
        return Result<bool>.Success(true);
    }
}
