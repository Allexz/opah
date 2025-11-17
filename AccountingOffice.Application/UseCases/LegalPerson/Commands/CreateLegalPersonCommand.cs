using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace AccountingOffice.Application.UseCases.Legal.Commands;

/// <summary>
/// Command para criação de uma pessoa jurídica (LegalPerson).
/// </summary>
public sealed record CreateLegalPersonCommand : ICommand<Result<Guid>>
{
    public CreateLegalPersonCommand(Guid tenantId,
                                    string name,
                                    string document,
                                    string email,
                                    string phoneNumber,
                                    string legalName)
    {
        TenantId = tenantId;
        Name = name;
        Document = document;
        Email = email;
        PhoneNumber = phoneNumber;
        LegalName = legalName;
    }

    /// <summary>
    /// Identificador do tenant/empresa à qual a pessoa pertence.
    /// </summary>
    public Guid TenantId { get; init; }

    /// <summary>
    /// Nome completo da pessoa jurídica.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// CNPJ da pessoa jurídica (documento).
    /// </summary>
    public string Document { get; init; } = string.Empty;

    /// <summary>
    /// E-mail de contato da pessoa jurídica.
    /// </summary>
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")] 
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Número de telefone de contato.
    /// </summary>
    public string PhoneNumber { get; init; } = string.Empty;

    /// <summary>
    /// Razão social da pessoa jurídica.
    /// </summary>
    public string LegalName { get; init; } = string.Empty;
}
