using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Application.Interfaces.Queries;

/// <summary>
/// Interface para consultas de usuários.
/// </summary>
public interface IUserQuery
{
    /// <summary>
    /// Busca um usuário pelo seu Id e TenantId.
    /// </summary>
    /// <param name="id">Identificador do usuário.</param>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Usuário encontrado ou null se não existir.</returns>
    Task<User?> GetByIdAsync(int id, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todos os usuários de um tenant/empresa.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de usuários do tenant.</returns>
    Task<IEnumerable<User>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca um usuário pelo nome de usuário (UserName) e tenant.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="userName">Nome de usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Usuário encontrado ou null se não existir.</returns>
    Task<User?> GetByUserNameAsync(Guid tenantId, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca usuários ativos de um tenant/empresa.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de usuários ativos do tenant.</returns>
    Task<IEnumerable<User>> GetActiveByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

