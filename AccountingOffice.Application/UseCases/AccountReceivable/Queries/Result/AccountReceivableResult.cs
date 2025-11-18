using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Enums;

namespace AccountingOffice.Application.UseCases.AccountReceiv.Queries.Result;

public sealed record AccountReceivableResult(Guid id,
                                             Guid tenantId,
                                             string description,
                                             decimal ammount,
                                             DateTime dueDate,
                                             DateTime issueDate,
                                             int status,
                                             Person<Guid> customer,
                                             int payMethod,
                                             string invoiceNumber,
                                             DateTime? receivedDate = null);