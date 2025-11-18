using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.Installm.Commands;

public sealed record AddInstallmentCommand(Guid AccountId,
                                           Guid TenantId,
                                           int entryType,
                                           int InstallmentNumber,
                                           decimal Amount,
                                           DateTime DueDate) : ICommand<Result<bool>>;
 
