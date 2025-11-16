using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Enums;
using AccountingOffice.Domain.Core.Interfaces;

namespace AccountingOffice.Domain.Core.Aggregates;

public abstract class Person<TId> : IMultiTenantEntity<Guid>
{
    #region Propriedades
    public TId Id { get; protected set; }
    public Guid TenantId { get; protected set; }
    public string Name { get; private set; }
    public string Document { get; private set; }
    public PersonType Type { get; private set; }

    private DateTime _createdAt;
    public DateTime CreatedAt
    {
        get => _createdAt;
        set => _createdAt = DateTime.UtcNow;
    }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public bool Active { get; set; }
    #endregion

    #region Construtores
    /// <summary>
    /// Inicializador protegido para ORM.
    /// </summary>
    protected Person() { }

    /// <summary>
    /// Construtor protegido para inicialização de uma nova pessoa.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tenantId"></param>
    /// <param name="name"></param>
    /// <param name="document"></param>
    /// <param name="type"></param>
    /// <param name="email"></param>
    /// <param name="phone"></param>
    protected Person(TId id,
                     Guid tenantId,
                     string name,
                     string document,
                     PersonType type,
                     string email,
                     string phone)
    {
        Id = id;
        TenantId = tenantId;
        Name = name.Trim();
        Document = document.Trim();
        Type = type;
        Email = email.Trim();
        Phone = phone.Trim();
        _createdAt = DateTime.UtcNow;
    }

    #endregion

    #region Validação

    /// <summary>
    /// Validação dos parâmetros comuns para criação de pessoa.
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="name"></param>
    /// <param name="document"></param>
    /// <param name="email"></param>
    /// <param name="phone"></param>
    /// <returns></returns>
    protected static Result ValidatePersonParameters(
        Guid tenantId,
        string name,
        string document,
        string email,
        string phone)
    {
        if (tenantId == Guid.Empty)
            return Result.Failure("TenantId é obrigatório.");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Nome é requerido.");

        if (string.IsNullOrWhiteSpace(document))
            return Result.Failure("Documento é requerido.");

        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure("E-mail é requerido.");

        if (string.IsNullOrWhiteSpace(phone))
            return Result.Failure("Telefone é requerido.");

        return Result.Success();
    }
    #endregion

    #region Alteração de estado

    /// <summary>
    /// Altera o nome da pessoa.
    /// </summary>
    /// <param name="newName"></param>
    /// <returns></returns>
    public Result ChangeName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return Result.Failure("Nome é requerido.");

        Name = newName.Trim();
        return Result.Success();
    }

    /// <summary>
    /// Altera o e-mail da pessoa.
    /// </summary>
    /// <param name="newEmail"></param>
    /// <returns></returns>
    public Result ChangeEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
            return Result.Failure("E-mail é requerido.");

        Email = newEmail.Trim();
        return Result.Success();
    }

    /// <summary>
    /// Altera o telefone da pessoa.
    /// </summary>
    /// <param name="newPhone"></param>
    /// <returns></returns>
    public Result ChangePhone(string newPhone)
    {
        if (string.IsNullOrWhiteSpace(newPhone))
            return Result.Failure("Telefone é requerido.");

        Phone = newPhone.Trim();
        return Result.Success();
    }

    #endregion
}