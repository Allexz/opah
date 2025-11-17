using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.Individual.Commands;

public sealed record UpdateIndividualPersonCommand: ICommand<Result<bool>>
{
    public UpdateIndividualPersonCommand(Guid tenantId,
                                         Guid id,
                                         string name = "",
                                         string email = "",
                                         string phoneNumber = "",
                                         int maritalStatus = 0)
    {
        TenantId = tenantId;
        Id = id;
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        MaritalStatus = maritalStatus;
    }

    public bool HasName => !string.IsNullOrWhiteSpace(Name);
    public bool HasEmail => !string.IsNullOrWhiteSpace(Email);
    public bool HasPhoneNumber => !string.IsNullOrWhiteSpace(PhoneNumber);
    public bool HasMaritalStatus => MaritalStatus > 0;


    /// <summary>
    /// Identificador do Tenant.
    /// </summary>
    public Guid TenantId { get; init; }

    /// <summary>
    /// Identificador da pessoa.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Nome completo da pessoa física.
    /// </summary>
    public string Name { get; init; }  

    /// <summary>
    /// E-mail de contato da pessoa física.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Número de telefone de contato.
    /// </summary>
    public string PhoneNumber { get; init; } = string.Empty;

    /// <summary>
    /// Estado civil da pessoa física.
    /// </summary>
    public int MaritalStatus { get; init; }
}
