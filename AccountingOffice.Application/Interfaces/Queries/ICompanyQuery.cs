using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Application.Interfaces.Queries;

/// <summary>
/// Interface para consultas de empresas/tenants.
/// </summary>
public interface ICompanyQuery
{
    /// <summary>
    /// Busca uma empresa pelo seu Id.
    /// </summary>
    /// <param name="id">Identificador da empresa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Empresa encontrada ou null se não existir.</returns>
    Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todas as empresas ativas.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de empresas ativas.</returns>
    Task<IEnumerable<Company>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca uma empresa pelo documento (CNPJ).
    /// </summary>
    /// <param name="document">Documento da empresa (CNPJ).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Empresa encontrada ou null se não existir.</returns>
    Task<Company?> GetByDocumentAsync(string document, CancellationToken cancellationToken = default);
}

