using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.Cia.Queries.Result;

namespace AccountingOffice.Application.UseCases.Cia.Queries;

public sealed record class GetCompanyByIdQuery(int CompanyId) : IQuery<CompanyResult?>;
