using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.AccountPay.Queries.Result;

namespace AccountingOffice.Application.UseCases.AccountPay.Queries;

public sealed record GetAccountPayByIdQuery(Guid Id, Guid TenantId) : IQuery<Result<AccountPayableResult?>>;
 