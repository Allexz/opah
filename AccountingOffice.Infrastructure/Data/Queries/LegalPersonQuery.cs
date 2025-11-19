using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Domain.Core.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace AccountingOffice.Infrastructure.Data.Queries;

/// <summary>
/// Serviço de consulta para pessoas jurídicas.
/// </summary>
public class LegalPersonQuery : ILegalPersonQuery
{
    private readonly AccountingOfficeDbContext _dbContext;

    public LegalPersonQuery(AccountingOfficeDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Busca uma pessoa jurídica pelo seu ID e TenantId.
    /// </summary>
    public async Task<LegalPerson?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Persons.OfType<LegalPerson>()
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId, cancellationToken);
    }

    /// <summary>
    /// Busca todas as pessoas jurídicas de um tenant.
    /// </summary>
    public async Task<IEnumerable<LegalPerson>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Persons.OfType<LegalPerson>()
            .Where(p => p.TenantId == tenantId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca uma pessoa jurídica pelo documento (CNPJ) e tenant.
    /// </summary>
    public async Task<LegalPerson?> GetByDocumentAsync(Guid tenantId, string document, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Persons.OfType<LegalPerson>()
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Document == document, cancellationToken);
    }

    /// <summary>
    /// Busca pessoas jurídicas ativas de um tenant.
    /// </summary>
    public async Task<IEnumerable<LegalPerson>> GetActiveByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Persons.OfType<LegalPerson>()
            .Where(p => p.TenantId == tenantId && p.Active)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }
}
