using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.AccountPay.Commands;

/// <summary>
/// Command para exclusão de uma conta a pagar (AccountPayable).
/// </summary>
public sealed record DeleteAccountPayableCommand(Guid Id, Guid TenantId) : ICommand<Result<bool>>;

