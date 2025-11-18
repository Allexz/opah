using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.Cia.Commands;

public sealed record ToggleCompanyActiveStatusCommand(Guid Id, bool Active) : ICommand<Result<bool>>;
