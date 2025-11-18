using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.Installments.Commands;

public sealed record ChangeInstallmentStatusCommand(Guid AccountId,
                                                    Guid TenantId,
                                                    int InstallmentNumber,
                                                    int accountStatus) : ICommand<Result<bool>>;
