using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.AccountPay.Queries.Result;

namespace AccountingOffice.Application.UseCases.AccountPay .Queries;

public sealed record GetAccountByRelatedPartQuery (Guid RelatedPartId, Guid TenantId,  int PageNum = 1, int PageSize = 20) : IQuery<Result<IEnumerable<AccountPayableResult>>>;
 