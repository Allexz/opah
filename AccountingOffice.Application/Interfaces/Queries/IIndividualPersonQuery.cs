using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Application.Interfaces.Queries;

/// <summary>
/// Interface para consultas de pessoas físicas.
/// </summary>
public interface IIndividualPersonQuery
{
    /// <summary>
    /// Busca uma pessoa física pelo seu Id e TenantId.
    /// </summary>
    /// <param name="id">Identificador da pessoa física.</param>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Pessoa física encontrada ou null se não existir.</returns>
    Task<IndividualPerson?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todas as pessoas físicas de um tenant/empresa.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de pessoas físicas do tenant.</returns>
    Task<IEnumerable<IndividualPerson>> GetByTenantIdAsync(Guid tenantId, int PageNum = 1, int PageSize =20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca uma pessoa física pelo documento (CPF) e tenant.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="document">CPF da pessoa física.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Pessoa física encontrada ou null se não existir.</returns>
    Task<IndividualPerson?> GetByDocumentAsync(Guid tenantId, string document, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca pessoas físicas ativas de um tenant/empresa.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de pessoas físicas ativas do tenant.</returns>
    Task<IEnumerable<IndividualPerson>> GetActiveByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
