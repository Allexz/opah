using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.Installm.Commands;

public sealed record DeleteInstallmentCommand(Guid AccountId,
                                              Guid TenantId,
                                              int InstallmentNumber,
                                              DateTime PaymentDate) :ICommand<Result<bool>>;
