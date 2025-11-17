using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Domain.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace AccountingOffice.Application.UseCases.Individual.Commands;

public sealed record UpdateIndividualPersonCommand: ICommand<Result<bool>>
{
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
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// E-mail de contato da pessoa física.
    /// </summary>
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Número de telefone de contato.
    /// </summary>
    public string PhoneNumber { get; init; } = string.Empty;

    /// <summary>
    /// Estado civil da pessoa física.
    /// </summary>
    public MaritalStatus MaritalStatus { get; init; }
}
