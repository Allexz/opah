using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.AccountReceiv.Queries.Result;

namespace AccountingOffice.Application.UseCases.AccountReceiv.Queries;

public sealed record GetAccountReceivByIdQuery(Guid Id, Guid TenantId) 
    : IQuery<Result<AccountReceivableResult?>>;
