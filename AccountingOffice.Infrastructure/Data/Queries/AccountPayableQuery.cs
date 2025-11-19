using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AccountingOffice.Infrastructure.Data.Queries;

/// <summary>
/// Serviço de consulta para contas a pagar.
/// </summary>
public class AccountPayableQuery : IAccountPayableQuery
{
    private readonly AccountingOfficeDbContext _dbContext;

    public AccountPayableQuery(AccountingOfficeDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Busca uma conta a pagar pelo seu ID e TenantId.
    /// </summary>
    public async Task<AccountPayable?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccountsPayable
            .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId, cancellationToken);
    }

    /// <summary>
    /// Busca todas as contas a pagar de um tenant com paginação.
    /// </summary>
    public async Task<IEnumerable<AccountPayable>> GetByTenantIdAsync(Guid tenantId, int pageNum = 1, int PageSize = 20, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccountsPayable
            .Where(a => a.TenantId == tenantId)
            .OrderBy(a => a.DueDate)
            .Skip((pageNum - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca contas a pagar por status e tenant com paginação.
    /// </summary>
    public async Task<IEnumerable<AccountPayable>> GetByStatusAsync(Guid tenantId, AccountStatus status, int pageNum = 1, int PageSize = 20, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccountsPayable
            .Where(a => a.TenantId == tenantId && a.Status == status)
            .OrderBy(a => a.DueDate)
            .Skip((pageNum - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca contas a pagar por período de vencimento e tenant com paginação.
    /// </summary>
    public async Task<IEnumerable<AccountPayable>> GetByDueDateRangeAsync(Guid tenantId, DateTime startDate, DateTime endDate, int pageNum = 1, int PageSize = 20, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccountsPayable
            .Where(a => a.TenantId == tenantId && a.DueDate >= startDate && a.DueDate <= endDate)
            .OrderBy(a => a.DueDate)
            .Skip((pageNum - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca contas a pagar por data de emissão e tenant com paginação.
    /// </summary>
    public async Task<IEnumerable<AccountPayable>> GetByIssueDateAsync(Guid tenantId, DateTime startDate, DateTime endDate, int pageNum = 1, int PageSize = 20, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccountsPayable
            .Where(a => a.TenantId == tenantId && a.IssueDate >= startDate && a.IssueDate <= endDate)
            .OrderBy(a => a.IssueDate)
            .Skip((pageNum - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca contas a pagar por parceiro de negócios com paginação.
    /// </summary>
    public async Task<IEnumerable<AccountPayable>> GetByRelatedPartyId(Guid tenantId, Guid RelatedPartyId, int pageNum = 1, int PageSize = 20, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccountsPayable
            .Where(a => a.TenantId == tenantId && a.RelatedParty.Id == RelatedPartyId)
            .OrderBy(a => a.DueDate)
            .Skip((pageNum - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);
    }
}
