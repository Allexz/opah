using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.UseCases.Individual.Queries;
using AccountingOffice.Application.UseCases.Individual.Queries.Result;
using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Application.UseCases.Individual.QueryHandler;

public class IndividualQueryHandler :
    IQueryHandler<GetIndividualByIdQuery, Result<IndividualPersonResult?>>,
    IQueryHandler<GetIndividualByDocument,Result<IndividualPersonResult?>>,
    IQueryHandler<GetIndividualByTenantId, Result<IEnumerable<IndividualPersonResult>>>
{
    private readonly IIndividualPersonQuery _personQuery;

    public IndividualQueryHandler(IIndividualPersonQuery personQuery)
    {
        _personQuery = personQuery;
    }

    public async Task<Result<IndividualPersonResult?>> Handle(GetIndividualByIdQuery query, CancellationToken cancellationToken)
    {
        IndividualPerson? person = await _personQuery.GetByIdAsync(query.Id, query.TenantId);
        if (person is null)
        {
            return Result<IndividualPersonResult?>.Failure("Registro não localizado para os dados informados.");
        }
        return Result<IndividualPersonResult?>.Success(new(query.Id,
                                            query.TenantId,
                                            person.Name,
                                            person.Document,
                                            (int)person.Type,
                                            person.Email,
                                            person.Phone,
                                            (int)person.MaritalStatus));
    }

    public async Task<Result<IndividualPersonResult?>> Handle(GetIndividualByDocument query, CancellationToken cancellationToken)
    {
        IndividualPerson? person = await _personQuery.GetByDocumentAsync(query.TenantId, query.document);
        if (person is null)
        {
            return Result<IndividualPersonResult?>.Failure("Registro não localizado para os dados informados.");
        }
        return Result<IndividualPersonResult?>.Success(new(person.Id,
                                            query.TenantId,
                                            person.Name,
                                            person.Document,
                                            (int)person.Type,
                                            person.Email,
                                            person.Phone,
                                            (int)person.MaritalStatus));
    }

    public async Task<Result<IEnumerable<IndividualPersonResult>>> Handle(GetIndividualByTenantId query, CancellationToken cancellationToken)
    {
        IEnumerable<IndividualPerson> personList = await _personQuery.GetByTenantIdAsync(query.TenantId, query.PageNum, query.PageSize, cancellationToken);
        if (personList.Any())
        {
            return Result<IEnumerable<IndividualPersonResult>>.Failure("Não foram encontrados registros para o parâmetro informado");
        }

        return Result<IEnumerable<IndividualPersonResult>>.Success(personList.Select(x => MapToIndividualPersonResult(x) ));
    }

    private static IndividualPersonResult MapToIndividualPersonResult(IndividualPerson person)
    {
        return new(person.Id,
                   person.TenantId,
                   person.Name,
                   person.Document,
                   (int)person.Type,
                   person.Email,
                   person.Phone,
                   (int)person.MaritalStatus);
    }
}
