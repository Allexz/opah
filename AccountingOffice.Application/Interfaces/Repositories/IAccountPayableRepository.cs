using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Application.Interfaces.Repositories;

/// <summary>
/// Interface para o repositório de contas a pagar.
/// </summary>
public interface IAccountPayableRepository
{
    /// <summary>
    /// Adiciona uma nova conta a pagar ao DB
    /// </summary>
    /// <param name="accountPayable"></param>
    /// <returns></returns>
    Task<AccountPayable> CreateAsync(AccountPayable accountPayable);

    /// <summary>
    /// Atualiza uma conta a pagar existente no DB
    /// </summary>
    /// <param name="accountPayable"></param>
    /// <returns></returns>
    Task<AccountPayable> UpdateAsync(AccountPayable accountPayable);

    /// <summary>
    /// Exclui uma conta a pagar do DB pelo seu Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Guid id);
}

