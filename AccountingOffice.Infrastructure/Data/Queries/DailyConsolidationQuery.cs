using AccountingOffice.Application.Interfaces.Queries;
using Microsoft.EntityFrameworkCore;

namespace AccountingOffice.Infrastructure.Data.Queries;

/// <summary>
/// Serviço de consulta para consolidação diária.
/// </summary>
public class DailyConsolidationQuery : IDailyConsolidationQuery
{
    private readonly AccountingOfficeDbContext _dbContext;

    public DailyConsolidationQuery(AccountingOfficeDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Busca o movimento diário consolidado por TenantId e data.
    /// </summary>
    public async Task<DailyConsolidationData?> GetDailyConsolidationAsync(Guid tenantId, DateTime date, CancellationToken cancellationToken = default)
    {
        // Normalizar a data para o início do dia (00:00:00)
        var startDate = date.Date;
        var endDate = startDate.AddDays(1).AddTicks(-1); // Fim do dia (23:59:59.9999999)

        // Buscar total de contas a pagar do dia (por data de emissão)
        var payableQuery = _dbContext.AccountsPayable
            .Where(a => a.TenantId == tenantId && 
                       a.IssueDate >= startDate && 
                       a.IssueDate <= endDate);

        var totalPayable = await payableQuery.SumAsync(a => (decimal?)a.Ammount, cancellationToken) ?? 0m;
        var payableCount = await payableQuery.CountAsync(cancellationToken);

        // Buscar total de contas a receber do dia (por data de emissão)
        var receivableQuery = _dbContext.AccountsReceivable
            .Where(a => a.TenantId == tenantId && 
                       a.IssueDate >= startDate && 
                       a.IssueDate <= endDate);

        var totalReceivable = await receivableQuery.SumAsync(a => (decimal?)a.Ammount, cancellationToken) ?? 0m;
        var receivableCount = await receivableQuery.CountAsync(cancellationToken);

        // Calcular saldo (receber - pagar)
        var balance = totalReceivable - totalPayable;

        // Se não houver movimentação, retornar null
        if (payableCount == 0 && receivableCount == 0)
        {
            return null;
        }

        return new DailyConsolidationData(
            tenantId,
            startDate,
            totalPayable,
            totalReceivable,
            balance,
            payableCount,
            receivableCount);
    }
}

