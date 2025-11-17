using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace AccountingOffice.Application.UseCases.Legal.Commands;

/// <summary>
/// Command para atualização de uma pessoa jurídica (LegalPerson).
/// </summary>
public sealed record UpdateLegalPersonCommand : ICommand<Result<bool>>
{
    /// <summary>
    /// Identificador do Tenant.
    /// </summary>
    public Guid TenantId { get; init; }

    /// <summary>
    /// Identificador da pessoa jurídica.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Nome completo da pessoa jurídica.
    /// </summary>
    public string Name { get; init; } = string.Empty;

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
