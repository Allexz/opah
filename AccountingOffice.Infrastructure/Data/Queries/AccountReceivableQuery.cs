using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Domain.Core.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace AccountingOffice.Infrastructure.Data.Queries;

/// <summary>
/// Serviço de consulta para contas a receber.
/// </summary>
public class AccountReceivableQuery : IAccountReceivableQuery
{
    private readonly AccountingOfficeDbContext _dbContext;

    public AccountReceivableQuery(AccountingOfficeDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Busca uma conta a receber pelo seu ID e TenantId.
    /// </summary>
    public async Task<AccountReceivable?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccountsReceivable
            .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId, cancellationToken);
    }

    /// <summary>
    /// Busca todas as contas a receber de um tenant com paginação.
    /// </summary>
    public async Task<IEnumerable<AccountReceivable>> GetByTenantIdAsync(Guid tenantId, int pageNum = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccountsReceivable
            .Where(a => a.TenantId == tenantId)
            .OrderBy(a => a.DueDate)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca contas a receber por período de emissão e tenant com paginação.
    /// </summary>
    public async Task<IEnumerable<AccountReceivable>> GetByIssueDateAsync(Guid tenantId, DateTime startDate, DateTime endDate, int pageNum = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccountsReceivable
            .Where(a => a.TenantId == tenantId && a.IssueDate >= startDate && a.IssueDate <= endDate)
            .OrderBy(a => a.IssueDate)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca uma conta a receber pelo número da nota fiscal e tenant.
    /// </summary>
    public async Task<AccountReceivable?> GetByInvoiceNumberAsync(Guid tenantId, string invoiceNumber, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccountsReceivable
            .FirstOrDefaultAsync(a => a.TenantId == tenantId && a.InvoiceNumber == invoiceNumber, cancellationToken);
    }

    /// <summary>
    /// Busca contas a receber por parte relacionada e tenant com paginação.
    /// </summary>
    public async Task<IEnumerable<AccountReceivable>> GetByRelatedPartyAsync(Guid tenantId, Guid RelatedPartId, int pageNum = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccountsReceivable
            .Where(a => a.TenantId == tenantId && a.RelatedParty.Id == RelatedPartId)
            .OrderBy(a => a.DueDate)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
