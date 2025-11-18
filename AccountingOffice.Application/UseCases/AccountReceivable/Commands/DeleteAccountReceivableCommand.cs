using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.AccountReceiv.Commands;

public sealed record DeleteAccountReceivableCommand(Guid Id, Guid TenantId) : ICommand<Result<bool>>;
