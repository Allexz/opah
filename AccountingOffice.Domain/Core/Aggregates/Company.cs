namespace AccountingOffice.Domain.Core.Aggregates;

/// <summary>
/// Representa uma empresa/tenant no sistema multi-empresas
/// </summary>
public class Company
{
    public Company(
        Guid id,
        string name,
        string document,
        string email,
        string phone,
        bool active = true)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome da empresa é requerido.", nameof(name));

        if (string.IsNullOrWhiteSpace(document))
            throw new ArgumentException("Documento da empresa é requerido.", nameof(document));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-mail da empresa é requerido.", nameof(email));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Telefone da empresa é requerido.", nameof(phone));

        Id = id;
        Name = name.Trim();
        Document = document.Trim();
        Email = email.Trim();
        Phone = phone.Trim();
        Active = active;
        CreatedAt = DateTime.UtcNow;
    }

    protected Company() { } // Para ORM

    public Guid Id { get; protected set; }
    public string Name { get; private set; } = string.Empty;
    public string Document { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; protected set; }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome da empresa é requerido.", nameof(name));
        Name = name.Trim();
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-mail da empresa é requerido.", nameof(email));
        Email = email.Trim();
    }

    public void UpdatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Telefone da empresa é requerido.", nameof(phone));
        Phone = phone.Trim();
    }

    public void Activate() => Active = true;
    public void Deactivate() => Active = false;
}

