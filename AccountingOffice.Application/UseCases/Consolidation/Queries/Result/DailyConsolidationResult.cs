namespace AccountingOffice.Application.UseCases.Consolidation.Queries.Result;

public sealed record DailyConsolidationResult(
    Guid TenantId,
    DateTime Date,
    decimal TotalPayable,
    decimal TotalReceivable,
    decimal Balance,
    int PayableCount,
    int ReceivableCount);


