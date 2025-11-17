using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Interfaces;

namespace AccountingOffice.Domain.Core.Aggregates;

/// <summary>
/// Representa um usuário do sistema vinculado a uma empresa (Company).
/// Seguindo DDD, User é uma entidade que pertence ao Aggregate Root Company.
/// </summary>
public class User : IMultiTenantEntity<Guid>
{
    #region Propriedades

    public int Id { get; protected set; }

    /// <summary>
    /// Identificador da empresa à qual o usuário pertence.
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

    #endregion

    #region Construtores
    private User(
    int id,
    Guid companyId,
    string userName,
    string password)
    {
        Id = id;
        TenantId = companyId; // CompanyId é o TenantId
        UserName = userName.Trim();
        Password = password; // Em produção, deve ser hash
        CreatedAt = DateTime.UtcNow;
        Active = true;
    }

    protected User() { } // Para ORM

    #endregion

    #region Validação
    private static DomainResult ValidateCreationParameters(Guid companyId, string userName, string password)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return DomainResult.Failure("UserName é obrigatório.");

        if (string.IsNullOrWhiteSpace(password))
            return DomainResult.Failure("Senha é obrigatório.");

        if (companyId == Guid.Empty)
            return DomainResult.Failure("CompanyId é obrigatório.");

        return DomainResult.Success();
    }
    #endregion

    #region Alteração de estado
    public static DomainResult<User> Create(
        int id,
        Guid companyId,
        string userName,
        string password)
    {
        DomainResult? validationResult = ValidateCreationParameters(companyId, userName, password);
        if (validationResult.IsFailure)
            return DomainResult<User>.Failure(validationResult.Error);

        return DomainResult<User>.Success(new User(id, companyId, userName, password));
    }

    /// <summary>
    /// Atualiza o nome de usuário.
    /// </summary>
    public void UpdateUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("UserName é obrigatório.", nameof(userName));
        UserName = userName.Trim();
    }

    /// <summary>
    /// Atualiza a senha do usuário.
    /// </summary>
    public DomainResult UpdatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            DomainResult.Failure("Password é obrigatório.");

        Password = password;
        return DomainResult.Success();
    }

    /// <summary>
    /// Ativa o usuário.
    /// </summary>
    public void Activate() => Active = true;

    /// <summary>
    /// Desativa o usuário.
    /// </summary>
    public void Deactivate() => Active = false;
    #endregion
}

