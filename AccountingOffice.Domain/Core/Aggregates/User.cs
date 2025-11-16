using AccountingOffice.Domain.Core.Interfaces;

namespace AccountingOffice.Domain.Core.Aggregates;

/// <summary>
/// Representa um usuário do sistema vinculado a uma empresa (Company).
/// Seguindo DDD, User é uma entidade que pertence ao Aggregate Root Company.
/// </summary>
public class User : IMultiTenantEntity<Guid>
{
    public User(
        int id,
        Guid companyId,
        string userName,
        string password)
    {
        if (id <= 0)
            throw new ArgumentException("Id deve ser maior que zero.", nameof(id));

        if (companyId == Guid.Empty)
            throw new ArgumentException("CompanyId é obrigatório.", nameof(companyId));

        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("UserName é obrigatório.", nameof(userName));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password é obrigatório.", nameof(password));

        if (userName.Length < 3)
            throw new ArgumentException("UserName deve ter pelo menos 3 caracteres.", nameof(userName));

        if (password.Length < 6)
            throw new ArgumentException("Password deve ter pelo menos 6 caracteres.", nameof(password));

        Id = id;
        TenantId = companyId; // CompanyId é o TenantId
        UserName = userName.Trim();
        Password = password; // Em produção, deve ser hash
        CreatedAt = DateTime.UtcNow;
        Active = true;
    }

    protected User() { } // Para ORM

    public int Id { get; protected set; }
    
    /// <summary>
    /// Identificador da empresa à qual o usuário pertence.
    /// Implementa IMultiTenantEntity para manter consistência com outras entidades.
    /// </summary>
    public Guid TenantId { get; protected set; }

    /// <summary>
    /// Nome de usuário único dentro do contexto da empresa.
    /// </summary>
    public string UserName { get; private set; } = string.Empty;

    /// <summary>
    /// Senha do usuário (deve ser armazenada como hash em produção).
    /// </summary>
    public string Password { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; protected set; }
    public bool Active { get; private set; }

    /// <summary>
    /// Atualiza o nome de usuário.
    /// </summary>
    public void UpdateUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("UserName é obrigatório.", nameof(userName));

        if (userName.Length < 3)
            throw new ArgumentException("UserName deve ter pelo menos 3 caracteres.", nameof(userName));

        UserName = userName.Trim();
    }

    /// <summary>
    /// Atualiza a senha do usuário.
    /// </summary>
    public void UpdatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password é obrigatório.", nameof(password));

        if (password.Length < 6)
            throw new ArgumentException("Password deve ter pelo menos 6 caracteres.", nameof(password));

        Password = password; // Em produção, deve ser hash
    }

    /// <summary>
    /// Ativa o usuário.
    /// </summary>
    public void Activate() => Active = true;

    /// <summary>
    /// Desativa o usuário.
    /// </summary>
    public void Deactivate() => Active = false;
}

