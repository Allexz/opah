using AccountingOffice.Domain.Core.Common;

namespace AccountingOffice.Domain.Core.Aggregates;

/// <summary>
/// Representa uma empresa/tenant no sistema multi-empresas
/// </summary>
public class Company
{
    #region Propriedades
    public Guid Id { get; protected set; }
    public string Name { get; private set; }  
    public string Document { get; private set; } 
    public string Email { get; private set; }  
    public string Phone { get; private set; }  
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

        List<string> errors = new();    

        if (string.IsNullOrWhiteSpace(name))
            errors.Add("Nome da empresa é requerido.");

        if (string.IsNullOrWhiteSpace(document))
            errors.Add("Documento da empresa é requerido.");

        if (string.IsNullOrWhiteSpace(email))
            errors.Add("E-mail da empresa é requerido.");

        if (string.IsNullOrWhiteSpace(phone))
            errors.Add("Telefone da empresa é requerido.");

        if (errors.Any())
            return DomainResult<Company>.Failure(string.Join("|", errors));

        return DomainResult<Company>.Success(
            new Company(
                id,
                name.Trim(),
                document.Trim(),
                email.Trim(),
                phone.Trim(),
                active));
    }



    public DomainResult ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return DomainResult.Failure("Nome da empresa é requerido.");
        Name = name.Trim();

        return DomainResult.Success();
    }

    public DomainResult ChangeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return DomainResult.Failure("E-mail da empresa é requerido.");
        Email = email.Trim();

        return DomainResult.Success();
    }

    public DomainResult ChangePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return DomainResult.Failure("Telefone da empresa é requerido.");
        Phone = phone.Trim();
        return DomainResult.Success();
    }

    public void Activate() => Active = true;
    public void Deactivate() => Active = false;
}

