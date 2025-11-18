using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.Individual.Queries.Result;

namespace AccountingOffice.Application.UseCases.Individual.Queries;

public sealed record GetIndividualByTenantId(Guid TenantId, int PageNum = 1, int PageSize = 20)
    : IQuery<Result<IEnumerable< IndividualPersonResult>>>;
