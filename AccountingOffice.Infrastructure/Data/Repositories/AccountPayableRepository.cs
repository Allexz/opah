using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório para contas a pagar.
/// </summary>
public class AccountPayableRepository : IAccountPayableRepository
{
    private readonly AccountingOfficeDbContext _dbContext;

    public AccountPayableRepository(AccountingOfficeDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Adiciona uma nova conta a pagar ao banco de dados.
    /// </summary>
    /// <param name="accountPayable">A conta a pagar a ser adicionada.</param>
    /// <returns>A conta a pagar adicionada.</returns>
    public async Task<AccountPayable> CreateAsync(AccountPayable accountPayable)
    {
        _dbContext.AccountsPayable.Add(accountPayable);
        await _dbContext.SaveChangesAsync();
        return accountPayable;
    }

    /// <summary>
    /// Atualiza uma conta a pagar existente no banco de dados.
    /// </summary>
    /// <param name="accountPayable">A conta a pagar a ser atualizada.</param>
    /// <returns>A conta a pagar atualizada.</returns>
    public async Task<AccountPayable> UpdateAsync(AccountPayable accountPayable)
    {
        _dbContext.AccountsPayable.Update(accountPayable);
        await _dbContext.SaveChangesAsync();
        return accountPayable;
    }

    /// <summary>
    /// Remove uma conta a pagar do banco de dados pelo seu ID.
    /// </summary>
    /// <param name="id">O ID da conta a pagar a ser removida.</param>
    /// <returns>True se a remoção foi bem-sucedida, false se não encontrada.</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _dbContext.AccountsPayable.FindAsync(id);
        if (entity == null) return false;

        _dbContext.AccountsPayable.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
