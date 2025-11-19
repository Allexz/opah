using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório para usuários.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AccountingOfficeDbContext _dbContext;

    public UserRepository(AccountingOfficeDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Adiciona um novo usuário ao banco de dados.
    /// </summary>
    /// <param name="user">O usuário a ser adicionado.</param>
    /// <returns>O usuário adicionado.</returns>
    public async Task<User> CreateAsync(User user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Atualiza um usuário existente no banco de dados.
    /// </summary>
    /// <param name="user">O usuário a ser atualizado.</param>
    /// <returns>O usuário atualizado.</returns>
    public async Task<User> UpdateAsync(User user)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Remove um usuário do banco de dados pelo seu ID.
    /// </summary>
    /// <param name="id">O ID do usuário a ser removido.</param>
    /// <returns>True se a remoção foi bem-sucedida, false se não encontrado.</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _dbContext.Users.FindAsync(id);
        if (entity == null) return false;

        _dbContext.Users.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
