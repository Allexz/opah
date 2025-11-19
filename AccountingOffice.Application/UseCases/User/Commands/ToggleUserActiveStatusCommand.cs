using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.Usr.Commands;

public sealed record ToggleUserActiveStatusCommand(int Id, Guid TenantId,bool status) : ICommand<Result<bool>>;
