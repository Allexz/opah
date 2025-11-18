using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.UseCases.Cia.Queries;
using AccountingOffice.Application.UseCases.Cia.Queries.Result;

namespace AccountingOffice.Application.UseCases.Cia.QueryHandler;

public class CompanyQueryHandler :
    IQueryHandler<GetCompanyByIdQuery, Result<CompanyResult?>>,
    IQueryHandler<GetCompanyByDocumentQuery, Result<CompanyResult?>>,
    IQueryHandler<GetAllCompaniesQuery, Result<IEnumerable<CompanyResult?>>>
{
    private readonly ICompanyQuery _companyQuery;

    public CompanyQueryHandler(ICompanyQuery companyQuery)
    {
        _companyQuery = companyQuery ?? throw new ArgumentNullException(nameof(companyQuery));
    }

    public async Task<Result<CompanyResult?>> Handle(GetCompanyByIdQuery query, CancellationToken cancellationToken)
    {
        var companyId = new Guid(query.CompanyId.ToString().PadLeft(32, '0'));
        var company = await _companyQuery.GetByIdAsync(companyId, cancellationToken);

        if (company is null)
            return Result<CompanyResult?>.Failure("Não foi localizado companhia para os parâmetros informados");

        return Result<CompanyResult?>.Success( MapToCompanyResult(company));
    }

    public async Task<Result<CompanyResult?>> Handle(GetCompanyByDocumentQuery query, CancellationToken cancellationToken)
    {
        var company = await _companyQuery.GetByDocumentAsync(query.Document, cancellationToken);

        if (company is null)
            return Result<CompanyResult?>.Failure("Não foi localizada companhia para os parâmetros informados");

        return Result<CompanyResult?>.Success( MapToCompanyResult(company));
    }

    public async Task<Result<IEnumerable<CompanyResult?>>> Handle(GetAllCompaniesQuery query, CancellationToken cancellationToken)
    {
        var companies = await _companyQuery.GetAllActiveAsync(cancellationToken);

        return Result<IEnumerable<CompanyResult?>>.Success(companies.Select(MapToCompanyResult));
    }

    private static CompanyResult MapToCompanyResult(Domain.Core.Aggregates.Company company)
    {
        return new CompanyResult(
            company.Id,
            company.Name,
            company.Document,
            company.Email,
            company.Phone,
            company.Active,
            company.CreatedAt);
    }
}
