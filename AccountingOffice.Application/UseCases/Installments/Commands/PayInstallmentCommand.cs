using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.Installments.Commands;

public sealed record PayInstallmentCommand(
    Guid AccountId,
    Guid TenantId,
    int InstallmentNumber,
    DateTime PaymentDate,
    decimal PaymentAmount) : ICommand<Result<bool>>;