using AccountingOffice.Domain.Core.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingOffice.Application.Interfaces.Queries;

public interface IPersonQuery
{
    /// <summary>
    /// Busca uma pessoa física ou jurídica pelo seu Id e TenantId.
    /// </summary>
    /// <param name="id">Identificador da pessoa física.</param>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Pessoa física encontrada ou null se não existir.</returns>
    Task<Person<Guid>?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todas as pessoas físicas e jurídicas de um tenant/empresa.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de pessoas físicas e jurídicas do tenant.</returns>
    Task<IEnumerable<Person<Guid>>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca uma pessoa física ou jurídica pelo documento  e tenant.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="document">CPF da pessoa física.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Pessoa física ou jurídica encontrada ou null se não existir.</returns>
    Task<Person<Guid>?> GetByDocumentAsync(Guid tenantId, string document, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca pessoas físicas e jurídicas ativas de um tenant/empresa.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de pessoas físicas e jurídicas ativas do tenant.</returns>
    Task<IEnumerable<Person<Guid>>> GetActiveByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
