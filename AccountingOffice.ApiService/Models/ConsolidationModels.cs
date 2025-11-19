namespace AccountingOffice.ApiService.Models;

public record DailyConsolidationView(
    Guid TenantId,
    DateTime Date,
    decimal TotalPayable,
    decimal TotalReceivable,
    decimal Balance,
    int PayableCount,
    int ReceivableCount);


