using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.UseCases.Consolidation.Queries;
using AccountingOffice.Application.UseCases.Consolidation.Queries.Result;

namespace AccountingOffice.Application.UseCases.Consolidation.QueryHandler;

public class DailyConsolidationQueryHandler : IQueryHandler<GetDailyConsolidationQuery, Result<DailyConsolidationResult?>>
{
    private readonly IDailyConsolidationQuery _dailyConsolidationQuery;

    public DailyConsolidationQueryHandler(IDailyConsolidationQuery dailyConsolidationQuery)
    {
        _dailyConsolidationQuery = dailyConsolidationQuery ?? throw new ArgumentNullException(nameof(dailyConsolidationQuery));
    }

    public async Task<Result<DailyConsolidationResult?>> Handle(GetDailyConsolidationQuery query, CancellationToken cancellationToken)
    {
        var consolidationData = await _dailyConsolidationQuery.GetDailyConsolidationAsync(
            query.TenantId, 
            query.Date, 
            cancellationToken);

        if (consolidationData is null)
        {
            return Result<DailyConsolidationResult?>.Success(null);
        }

        var result = new DailyConsolidationResult(
            consolidationData.TenantId,
            consolidationData.Date,
            consolidationData.TotalPayable,
            consolidationData.TotalReceivable,
            consolidationData.Balance,
            consolidationData.PayableCount,
            consolidationData.ReceivableCount);

        return Result<DailyConsolidationResult?>.Success(result);
    }
}

