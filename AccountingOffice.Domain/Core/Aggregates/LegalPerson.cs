using AccountingOffice.Domain.Core.Enums;

namespace AccountingOffice.Domain.Core.Aggregates;

public class LegalPerson : Person<Guid>
{
    public LegalPerson(
        Guid id,
        Guid tenantId,
        string name,
        string document,
        PersonType type,
        string email,
        string phoneNumber,
        string legalName)
        : base(id, tenantId, name, document, type, email, phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(legalName))
            throw new ArgumentException("Razão social não pode ser nula ou vazia.", nameof(legalName));

        LegalName = legalName;
    }
    public string LegalName { get; private set; }
}
