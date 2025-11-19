using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Domain.Core.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace AccountingOffice.Infrastructure.Data.Queries;

/// <summary>
/// Serviço de consulta para pessoas (físicas e jurídicas).
/// </summary>
public class PersonQuery : IPersonQuery
{
    private readonly AccountingOfficeDbContext _dbContext;

    public PersonQuery(AccountingOfficeDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Busca uma pessoa (física ou jurídica) pelo seu ID e TenantId.
    /// </summary>
    public async Task<Person<Guid>?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        Person<Guid>? person = await _dbContext.Persons.OfType<IndividualPerson>()
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId, cancellationToken);

        if (person != null) return person;

        person = await _dbContext.Persons.OfType<LegalPerson>()
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId, cancellationToken);

        return person;
    }

    /// <summary>
    /// Busca todas as pessoas (físicas e jurídicas) de um tenant.
    /// </summary>
    public async Task<IEnumerable<Person<Guid>>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var individuals = _dbContext.Persons.OfType<IndividualPerson>()
            .Where(p => p.TenantId == tenantId)
            .Cast<Person<Guid>>();

        var legals = _dbContext.Persons.OfType<LegalPerson>()
            .Where(p => p.TenantId == tenantId)
            .Cast<Person<Guid>>();

        var allPersons = individuals.Union(legals)
            .OrderBy(p => p.Name);

        return await allPersons.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca uma pessoa (física ou jurídica) pelo documento e tenant.
    /// </summary>
    public async Task<Person<Guid>?> GetByDocumentAsync(Guid tenantId, string document, CancellationToken cancellationToken = default)
    {
        Person<Guid>? person = await _dbContext.Persons.OfType<IndividualPerson>()
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Document == document, cancellationToken);

        if (person != null) return person;

        person = await _dbContext.Persons.OfType<LegalPerson>()
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Document == document, cancellationToken);

        return person;
    }

    /// <summary>
    /// Busca pessoas (físicas e jurídicas) ativas de um tenant.
    /// </summary>
    public async Task<IEnumerable<Person<Guid>>> GetActiveByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var individuals = _dbContext.Persons.OfType<IndividualPerson>()
            .Where(p => p.TenantId == tenantId && p.Active)
            .Cast<Person<Guid>>();

        var legals = _dbContext.Persons.OfType<LegalPerson>()
            .Where(p => p.TenantId == tenantId && p.Active)
            .Cast<Person<Guid>>();

        var allPersons = individuals.Union(legals)
            .OrderBy(p => p.Name);

        return await allPersons.ToListAsync(cancellationToken);
    }
}
