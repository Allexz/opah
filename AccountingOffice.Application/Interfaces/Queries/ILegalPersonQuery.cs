using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Application.Interfaces.Queries;

/// <summary>
/// Interface para consultas de pessoas jurídicas.
/// </summary>
public interface ILegalPersonQuery
{
    /// <summary>
    /// Busca uma pessoa jurídica pelo seu Id e TenantId.
    /// </summary>
    /// <param name="id">Identificador da pessoa jurídica.</param>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Pessoa jurídica encontrada ou null se não existir.</returns>
    Task<LegalPerson?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todas as pessoas jurídicas de um tenant/empresa.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de pessoas jurídicas do tenant.</returns>
    Task<IEnumerable<LegalPerson>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca uma pessoa jurídica pelo documento (CNPJ) e tenant.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="document">CNPJ da pessoa jurídica.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Pessoa jurídica encontrada ou null se não existir.</returns>
    Task<LegalPerson?> GetByDocumentAsync(Guid tenantId, string document, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca pessoas jurídicas ativas de um tenant/empresa.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de pessoas jurídicas ativas do tenant.</returns>
    Task<IEnumerable<LegalPerson>> GetActiveByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

