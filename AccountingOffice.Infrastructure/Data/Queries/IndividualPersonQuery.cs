using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Domain.Core.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace AccountingOffice.Infrastructure.Data.Queries;

/// <summary>
/// Serviço de consulta para pessoas físicas.
/// </summary>
public class IndividualPersonQuery : IIndividualPersonQuery
{
    private readonly AccountingOfficeDbContext _dbContext;

    public IndividualPersonQuery(AccountingOfficeDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Busca uma pessoa física pelo seu ID e TenantId.
    /// </summary>
    public async Task<IndividualPerson?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Persons.OfType<IndividualPerson>()
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId, cancellationToken);
    }

    /// <summary>
    /// Busca todas as pessoas físicas de um tenant com paginação.
    /// </summary>
    public async Task<IEnumerable<IndividualPerson>> GetByTenantIdAsync(Guid tenantId, int PageNum = 1, int PageSize = 20, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Persons.OfType<IndividualPerson>()
            .Where(p => p.TenantId == tenantId)
            .OrderBy(p => p.Name)
            .Skip((PageNum - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca uma pessoa física pelo documento (CPF) e tenant.
    /// </summary>
    public async Task<IndividualPerson?> GetByDocumentAsync(Guid tenantId, string document, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Persons.OfType<IndividualPerson>()
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Document == document, cancellationToken);
    }

    /// <summary>
    /// Busca pessoas físicas ativas de um tenant.
    /// </summary>
    public async Task<IEnumerable<IndividualPerson>> GetActiveByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Persons.OfType<IndividualPerson>()
            .Where(p => p.TenantId == tenantId && p.Active)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }
}
