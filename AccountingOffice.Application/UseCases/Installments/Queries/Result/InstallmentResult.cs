using AccountingOffice.Domain.Core.Enums;

namespace AccountingOffice.Application.UseCases.Installm.Queries.Result;

public sealed record InstallmentResult(int InstallmentNumber,
                                       decimal Amount,
                                       DateTime DueDate,
                                       int Status,
                                       DateTime? PaymentDate,
                                       EntryType Entrytype);
