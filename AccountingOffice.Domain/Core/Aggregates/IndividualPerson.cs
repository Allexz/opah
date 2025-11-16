using AccountingOffice.Domain.Core.Enums;

namespace AccountingOffice.Domain.Core.Aggregates;

public class IndividualPerson: Person<Guid>
{
    public IndividualPerson(
        Guid id,
        Guid tenantId,
        string name,
        string document,
        PersonType type,
        string email,
        string phoneNumber,
        MaritalStatus maritalStatus)
        : base(id, tenantId, name, document, type, email, phoneNumber)
    {
        if (!Enum.IsDefined(typeof(MaritalStatus), maritalStatus))
            throw new ArgumentException("Estado civil inválido.", nameof(maritalStatus));

        MaritalStatus = maritalStatus;
    }
    public MaritalStatus MaritalStatus { get; private set; }
}
