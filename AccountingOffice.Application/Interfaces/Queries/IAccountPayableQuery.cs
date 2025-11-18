using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Enums;

namespace AccountingOffice.Application.Interfaces.Queries;

/// <summary>
/// Interface para consultas de contas a pagar.
/// </summary>
public interface IAccountPayableQuery
{
    /// <summary>
    /// Busca uma conta a pagar pelo seu Id e TenantId.
    /// </summary>
    /// <param name="id">Identificador da conta a pagar.</param>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Conta a pagar encontrada ou null se não existir.</returns>
    Task<AccountPayable?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todas as contas a pagar de um tenant/empresa.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="pageNum"></param>
    /// <param name="PageSize"></param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de contas a pagar do tenant.</returns>
    Task<IEnumerable<AccountPayable>> GetByTenantIdAsync(Guid tenantId, int pageNum = 1, int PageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca contas a pagar por status e tenant.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="status">Status da conta a pagar.</param>
    /// <param name="pageNum"></param>
    /// <param name="PageSize"></param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de contas a pagar com o status especificado.</returns>
    Task<IEnumerable<AccountPayable>> GetByStatusAsync(Guid tenantId, AccountStatus status,int pageNum = 1, int PageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca contas a pagar por período de vencimento e tenant.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="startDate">Data inicial do período.</param>
    /// <param name="endDate">Data final do período.</param>
    /// <param name="pageNum"></param>
    /// <param name="PageSize"></param>

    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de contas a pagar no período especificado.</returns>
    Task<IEnumerable<AccountPayable>> GetByDueDateRangeAsync(Guid tenantId, DateTime startDate, DateTime endDate,  int pageNum = 1, int PageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca contas a pagar por data de emissão e tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="pageNum"></param>
    /// <param name="PageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<AccountPayable>> GetByIssueDateAsync(Guid tenantId, DateTime startDate, DateTime endDate, int pageNum = 1, int PageSize = 20, CancellationToken cancellationToken = default);




    /// <summary>
    /// Busca contas por parceiro de negócios
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="RelatedPartyId"></param>
    /// <param name="pageNum"></param>
    /// <param name="PageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<AccountPayable>> GetByRelatedPartyId(Guid tenantId,Guid RelatedPartyId, int pageNum = 1, int PageSize = 20, CancellationToken cancellationToken = default);
}

