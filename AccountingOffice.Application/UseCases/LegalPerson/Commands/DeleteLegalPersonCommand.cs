using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.Legal.Commands;

/// <summary>
/// Command para exclusão de uma pessoa jurídica (LegalPerson).
/// </summary>
public sealed record DeleteLegalPersonCommand(Guid Id, Guid TenantId) : ICommand<Result<bool>>;
