using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.Consolidation.Queries.Result;

namespace AccountingOffice.Application.UseCases.Consolidation.Queries;

public sealed record GetDailyConsolidationQuery(Guid TenantId, DateTime Date) : IQuery<Result<DailyConsolidationResult?>>;

