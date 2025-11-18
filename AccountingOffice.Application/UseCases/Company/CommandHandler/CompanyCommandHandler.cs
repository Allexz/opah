using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Application.UseCases.Cia.Commands;
using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Common;

namespace AccountingOffice.Application.UseCases.Cia.CommandHandler;

public class CompanyCommandHandler :
    ICommandHandler<CreateCompanyCommand, Result<Guid>>,
    ICommandHandler<UpdateCompanyCommand, Result<bool>>,
    ICommandHandler<DeleteCompanyCommand, Result<bool>>,
    ICommandHandler<ToggleCompanyActiveStatusCommand, Result<bool>>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyQuery _companyQuery;

    public CompanyCommandHandler(ICompanyRepository companyRepository, ICompanyQuery companyQuery)
    {
        _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        _companyQuery = companyQuery ?? throw new ArgumentNullException(nameof(companyQuery));
    }
    public async Task<Result<Guid>> Handle(CreateCompanyCommand command, CancellationToken cancellationToken)
    {
        DomainResult<Company> cia = Company.Create(Guid.NewGuid(),
                                                   command.Name,
                                                   command.Document,
                                                   command.Email,
                                                   command.Phone);

        if (cia.IsFailure)
            return Result<Guid>.Failure(cia.Error);

        await _companyRepository.CreateAsync(cia.Value);
        return Result<Guid>.Success(cia.Value.Id);
    }

    public async Task<Result<bool>> Handle(UpdateCompanyCommand command, CancellationToken cancellationToken)
    {
        Company? cia = await _companyQuery.GetByIdAsync(command.Id);
        if (cia is null)
        {
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
            return Result<bool>.Failure(string.Join("|", errors));

        await _companyRepository.UpdateAsync(cia);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> Handle(DeleteCompanyCommand command, CancellationToken cancellationToken)
    {
        Company? cia = await _companyQuery.GetByIdAsync(command.Id);
        if (cia is null)
        {
            return Result<bool>.Failure("Companhia não localizada.");
        }
        await _companyRepository.DeleteAsync(cia.Id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> Handle(ToggleCompanyActiveStatusCommand command, CancellationToken cancellationToken)
    {
        Company? cia = await _companyQuery.GetByIdAsync(command.Id);
        if (cia is null)
        {
            return Result<bool>.Failure("Companhia não localizada.");
        }

        if (command.Active)
            cia.Activate();
        else
            cia.Deactivate();
        
        return Result<bool>.Success(true);
    }
}
