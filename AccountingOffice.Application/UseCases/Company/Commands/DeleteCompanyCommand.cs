using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.Cia.Commands;

public sealed record DeleteCompanyCommand(Guid Id) : ICommand<Result<bool>>;
