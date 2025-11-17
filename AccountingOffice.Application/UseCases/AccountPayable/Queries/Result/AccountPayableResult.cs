using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Enums;

namespace AccountingOffice.Application.UseCases.AccountPay.Queries.Result;

public sealed record AccountPayableResult(Guid id,
                                          Guid tenantId,
                                          string description,
                                          decimal ammount,
                                          DateTime issueDate,
                                          DateTime dueDate,
                                          AccountStatus status,
                                          Person<Guid> supplier,
                                          PaymentMethod payMethod,
                                          DateTime? paymentDate = null);