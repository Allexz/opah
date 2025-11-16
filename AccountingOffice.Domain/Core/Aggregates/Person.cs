using AccountingOffice.Domain.Core.Enums;
using AccountingOffice.Domain.Core.Interfaces;

namespace AccountingOffice.Domain.Core.Aggregates;

public abstract class Person<TId> : IMultiTenantEntity<Guid>
{
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
    
    protected Person() { }

    protected Person(TId id,
                     Guid tenantId,
                     string name,
                     string document,
                     PersonType type,
                     string email,
                     string phone)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("TenantId é obrigatório.", nameof(tenantId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome é requerido.", nameof(name));

        if (string.IsNullOrWhiteSpace(document))
            throw new ArgumentException("Documento é requerido.", nameof(document));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-mail é requerido.", nameof(email));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Telefone é requerido.", nameof(phone));

        Id = id;
        TenantId = tenantId;
        Name = name.Trim();
        Document = document.Trim();
        Type = type;
        Email = email.Trim();
        Phone = phone.Trim();
        _createdAt = DateTime.UtcNow;
    }
}
