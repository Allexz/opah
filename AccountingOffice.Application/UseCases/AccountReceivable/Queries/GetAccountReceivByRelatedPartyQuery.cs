using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.AccountReceiv.Queries.Result;

namespace AccountingOffice.Application.UseCases.AccountReceiv.Queries;

public sealed record GetAccountReceivByRelatedPartyQuery(Guid RelatedPartId,
                                                         Guid TenantId,
                                                         int PageNum = 1,
                                                         int PageSize = 20): IQuery<Result<IEnumerable<AccountReceivableResult>>>;
