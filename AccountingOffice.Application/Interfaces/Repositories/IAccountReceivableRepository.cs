using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Application.Interfaces.Repositories;

/// <summary>
/// Interface para o repositório de contas a receber.
/// </summary>
public interface IAccountReceivableRepository
{
    /// <summary>
    /// Adiciona uma nova conta a receber ao DB
    /// </summary>
    /// <param name="accountReceivable"></param>
    /// <returns></returns>
    Task<AccountReceivable> CreateAsync(AccountReceivable accountReceivable);

    /// <summary>
    /// Atualiza uma conta a receber existente no DB
    /// </summary>
    /// <param name="accountReceivable"></param>
    /// <returns></returns>
    Task<AccountReceivable> UpdateAsync(AccountReceivable accountReceivable);

    /// <summary>
    /// Exclui uma conta a receber do DB pelo seu Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Guid id);
}

