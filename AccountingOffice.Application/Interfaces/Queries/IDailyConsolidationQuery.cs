namespace AccountingOffice.Application.Interfaces.Queries;

/// <summary>
/// Interface para consultas de consolidação diária.
/// </summary>
public interface IDailyConsolidationQuery
{
    /// <summary>
    /// Busca o movimento diário consolidado por TenantId e data.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="date">Data para consolidação (apenas a data será considerada).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Dados consolidados do movimento diário.</returns>
    Task<DailyConsolidationData?> GetDailyConsolidationAsync(Guid tenantId, DateTime date, CancellationToken cancellationToken = default);
}

/// <summary>
/// Dados consolidados do movimento diário.
/// </summary>
public sealed record DailyConsolidationData(
    Guid TenantId,
    DateTime Date,
    decimal TotalPayable,
    decimal TotalReceivable,
    decimal Balance,
    int PayableCount,
    int ReceivableCount);


