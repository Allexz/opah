using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Domain.Core.Enums;

namespace AccountingOffice.Application.UseCases.Individual.Commands;

/// <summary>
/// Command para criação de uma pessoa física (IndividualPerson).
/// </summary>
public sealed record CreateIndividualPersonCommand : ICommand<Result<Guid>>
{
    /// <summary>
    /// Identificador do tenant/empresa à qual a pessoa pertence.
    /// </summary>
    public Guid TenantId { get; init; }

    /// <summary>
    /// Nome completo da pessoa física.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// CPF da pessoa física (documento).
    /// </summary>
    public string Document { get; init; } = string.Empty;

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
    public MaritalStatus MaritalStatus { get; init; }
}
