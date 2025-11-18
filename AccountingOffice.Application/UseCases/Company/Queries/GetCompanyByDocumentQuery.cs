using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.Cia.Queries.Result;

namespace AccountingOffice.Application.UseCases.Cia.Queries;

public sealed record GetCompanyByDocumentQuery(string Document): IQuery<Result<CompanyResult?>>;
