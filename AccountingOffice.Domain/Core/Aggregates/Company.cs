using AccountingOffice.Domain.Core.Common;

namespace AccountingOffice.Domain.Core.Aggregates;

/// <summary>
/// Representa uma empresa/tenant no sistema multi-empresas
/// </summary>
public class Company
{
    #region Propriedades
    public Guid Id { get; protected set; }
    public string Name { get; private set; } = string.Empty;
    public string Document { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; protected set; }
    #endregion

    #region Construtor
    private Company(
        Guid id,
        string name,
        string document,
        string email,
        string phone,
        bool active = true)
    {
        Id = id;
        Name = name.Trim();
        Document = document.Trim();
        Email = email.Trim();
        Phone = phone.Trim();
        Active = active;
        CreatedAt = DateTime.UtcNow;
    }

    private Company() { } // Para ORM


    #endregion

    public static DomainResult<Company> Create(Guid id,
                                               string name,
                                               string document,
                                               string email,
                                               string phone,
                                               bool active = true)
    {
        if (string.IsNullOrWhiteSpace(name))
            return DomainResult<Company>.Failure("Nome da empresa é requerido.");

        if (string.IsNullOrWhiteSpace(document))
            return DomainResult<Company>.Failure("Documento da empresa é requerido.");

        if (string.IsNullOrWhiteSpace(email))
            return DomainResult<Company>.Failure("E-mail da empresa é requerido.");

        if (string.IsNullOrWhiteSpace(phone))
            return DomainResult<Company>.Failure("Telefone da empresa é requerido.");

        return DomainResult<Company>.Success(
            new Company(
                id,
                name.Trim(),
                document.Trim(),
                email.Trim(),
                phone.Trim(),
                active));
    }



    public DomainResult UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return DomainResult.Failure("Nome da empresa é requerido.");
        Name = name.Trim();

        return DomainResult.Success();
    }

    public DomainResult UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return DomainResult.Failure("E-mail da empresa é requerido.");
        Email = email.Trim();

        return DomainResult.Success();
    }

    public DomainResult UpdatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return DomainResult.Failure("Telefone da empresa é requerido.");
        Phone = phone.Trim();
        return DomainResult.Success();
    }

    public void Activate() => Active = true;
    public void Deactivate() => Active = false;
}

