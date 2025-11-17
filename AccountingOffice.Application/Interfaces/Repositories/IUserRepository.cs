using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Application.Interfaces.Repositories;

/// <summary>
/// Interface para o repositório de usuários.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Adiciona um novo usuário ao DB
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<User> CreateAsync(User user);

    /// <summary>
    /// Atualiza um usuário existente no DB
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<User> UpdateAsync(User user);

    /// <summary>
    /// Exclui um usuário do DB pelo seu Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(int id);
}

