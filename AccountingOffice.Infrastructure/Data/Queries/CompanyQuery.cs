using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Domain.Core.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace AccountingOffice.Infrastructure.Data.Queries;

/// <summary>
/// Serviço de consulta para empresas.
/// </summary>
public class CompanyQuery : ICompanyQuery
{
    private readonly AccountingOfficeDbContext _dbContext;

    public CompanyQuery(AccountingOfficeDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Busca uma empresa pelo seu ID.
    /// </summary>
    public async Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Companies
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <summary>
    /// Busca todas as empresas ativas.
    /// </summary>
    public async Task<IEnumerable<Company>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Companies
            .Where(c => c.Active)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca uma empresa pelo documento (CNPJ).
    /// </summary>
    public async Task<Company?> GetByDocumentAsync(string document, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Companies
            .FirstOrDefaultAsync(c => c.Document == document, cancellationToken);
    }
}
