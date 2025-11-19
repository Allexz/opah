using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.Individual.Commands;

public sealed record DeleteIndividualPersonCommand(Guid Id, Guid TenantId): ICommand<Result<bool>>;
