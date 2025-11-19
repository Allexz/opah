using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.Cia.Queries.Result;

namespace AccountingOffice.Application.UseCases.Cia.Queries;

public sealed record GetAllCompaniesQuery(int PageNumber, int PageSize): IQuery<Result<IEnumerable<CompanyResult>>>;
