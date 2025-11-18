using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Application.Interfaces.Queries;

/// <summary>
/// Interface para consultas de contas a receber.
/// </summary>
public interface IAccountReceivableQuery
{
    /// <summary>
    /// Busca uma conta a receber pelo seu Id e TenantId.
    /// </summary>
    /// <param name="id">Identificador da conta a receber.</param>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Conta a receber encontrada ou null se não existir.</returns>
    Task<AccountReceivable?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todas as contas a receber de um tenant/empresa.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de contas a receber do tenant.</returns>
    Task<IEnumerable<AccountReceivable>> GetByTenantIdAsync(Guid tenantId, int pageNum = 1, int pageSize = 20, CancellationToken cancellationToken = default);


    /// <summary>
    /// Busca contas a receber por período de vencimento e tenant.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="startDate">Data inicial do período.</param>
    /// <param name="endDate">Data final do período.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de contas a receber no período especificado.</returns>
    Task<IEnumerable<AccountReceivable>> GetByIssueDateAsync(Guid tenantId, DateTime startDate, DateTime endDate,int pageNum = 1, int pageSize =20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca uma conta a receber pelo número da nota fiscal e tenant.
    /// </summary>
    /// <param name="tenantId">Identificador do tenant/empresa.</param>
    /// <param name="invoiceNumber">Número da nota fiscal.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Conta a receber encontrada ou null se não existir.</returns>
    Task<AccountReceivable?> GetByInvoiceNumberAsync(Guid tenantId, string invoiceNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca contas a receber por parte relacionada e tenant.
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="RelatedPartId"></param>
    /// <param name="pageNum"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<AccountReceivable>> GetByRelatedPartyAsync(Guid tenantId, Guid RelatedPartId, int pageNum = 1, int pageSize = 20, CancellationToken cancellationToken = default);

}

