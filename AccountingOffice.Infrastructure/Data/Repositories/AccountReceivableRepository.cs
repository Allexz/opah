using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório para contas a receber.
/// </summary>
public class AccountReceivableRepository : IAccountReceivableRepository
{
    private readonly AccountingOfficeDbContext _dbContext;

    public AccountReceivableRepository(AccountingOfficeDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Adiciona uma nova conta a receber ao banco de dados.
    /// </summary>
    /// <param name="accountReceivable">A conta a receber a ser adicionada.</param>
    /// <returns>A conta a receber adicionada.</returns>
    public async Task<AccountReceivable> CreateAsync(AccountReceivable accountReceivable)
    {
        _dbContext.AccountsReceivable.Add(accountReceivable);
        await _dbContext.SaveChangesAsync();
        return accountReceivable;
    }

    /// <summary>
    /// Atualiza uma conta a receber existente no banco de dados.
    /// </summary>
    /// <param name="accountReceivable">A conta a receber a ser atualizada.</param>
    /// <returns>A conta a receber atualizada.</returns>
    public async Task<AccountReceivable> UpdateAsync(AccountReceivable accountReceivable)
    {
        _dbContext.AccountsReceivable.Update(accountReceivable);
        await _dbContext.SaveChangesAsync();
        return accountReceivable;
    }

    /// <summary>
    /// Remove uma conta a receber do banco de dados pelo seu ID.
    /// </summary>
    /// <param name="id">O ID da conta a receber a ser removida.</param>
    /// <returns>True se a remoção foi bem-sucedida, false se não encontrada.</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _dbContext.AccountsReceivable.FindAsync(id);
        if (entity == null) return false;

        _dbContext.AccountsReceivable.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
