using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.UseCases.AccountReceiv.Queries;
using AccountingOffice.Application.UseCases.AccountReceiv.Queries.Result;
using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Application.UseCases.AccountReceiv.QueryHandler;

public class AccountReceivableQueryHandler :
    IQueryHandler<GetAccountReceivByIdQuery, Result<AccountReceivableResult?>>,
    IQueryHandler<GetAccountReceivByIssueDateQuery, Result<IEnumerable<AccountReceivableResult>>>,
    IQueryHandler<GetAccountReceivByRelatedPartyQuery, Result<IEnumerable<AccountReceivableResult>>>,
    IQueryHandler<GetAccountReceivByTenantId, Result<IEnumerable<AccountReceivableResult>>>

{
    private readonly IAccountReceivableQuery _accountReceivableQuery;
    public AccountReceivableQueryHandler(IAccountReceivableQuery accountReceivableQuery)
    {
        _accountReceivableQuery = accountReceivableQuery ?? throw new ArgumentNullException(nameof(accountReceivableQuery));
    }
    public async Task<Result<AccountReceivableResult?>> Handle(GetAccountReceivByIdQuery query, CancellationToken cancellationToken)
    {
        AccountReceivable? account = await _accountReceivableQuery.GetByIdAsync(query.Id, query.TenantId, cancellationToken);
        if (account is null)
            return Result<AccountReceivableResult?>.Failure("Não foi encontrada conta a receber com este identificador");

        return Result<AccountReceivableResult?>.Success(MapToAccountReceivableResult(account));
    }

    public async Task<Result<IEnumerable<AccountReceivableResult>>> Handle(GetAccountReceivByIssueDateQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<AccountReceivable> accounts = await _accountReceivableQuery.GetByIssueDateAsync(query.TenantId,
                                                                                                    query.StartDate,
                                                                                                    query.EndDate,
                                                                                                    query.PageNum,
                                                                                                    query.PageSize,
                                                                                                    cancellationToken);

        if (!accounts.Any())
        {
            return Result<IEnumerable<AccountReceivableResult>>.Failure("Não foram encontradas contas para os parâmetros informados.");
        }
        return Result<IEnumerable<AccountReceivableResult>>.Success(accounts.Select(MapToAccountReceivableResult));
    }

    public async Task<Result<IEnumerable<AccountReceivableResult>>> Handle(GetAccountReceivByRelatedPartyQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<AccountReceivable> accounts = await _accountReceivableQuery.GetByRelatedPartyAsync(query.TenantId,
                                                                                                       query.RelatedPartId,
                                                                                                       query.PageNum,
                                                                                                       query.PageSize,
                                                                                                       cancellationToken);
        if (accounts is null)
        {
            return Result<IEnumerable<AccountReceivableResult>>.Failure("Não foram encontradas contas para os parâmetros informados.");
        }
        return Result<IEnumerable<AccountReceivableResult>>.Success(accounts.Select(MapToAccountReceivableResult));
    }

    public async Task<Result<IEnumerable<AccountReceivableResult>>> Handle(GetAccountReceivByTenantId query, CancellationToken cancellationToken)
    {
        IEnumerable<AccountReceivable> accounts = await _accountReceivableQuery.GetByTenantIdAsync(query.TenantId,query.PageNum, query.PageSize, cancellationToken);
        if (!accounts.Any())
        {
            return Result<IEnumerable<AccountReceivableResult>>.Failure("Não foram encontradas contas para os parâmetros informados.");
        }
         
        return Result<IEnumerable<AccountReceivableResult>>.Success(accounts.Select(MapToAccountReceivableResult));
    }

    private static AccountReceivableResult MapToAccountReceivableResult(Domain.Core.Aggregates.AccountReceivable account)
    {
        return new AccountReceivableResult(account.Id,
                                           account.TenantId,
                                           account.Description,
                                           account.Ammount,
                                           account.DueDate,
                                           account.IssueDate,
                                           (int)account.Status,
                                           account.RelatedParty,
                                           (int)account.PayMethod,
                                           account.InvoiceNumber,
                                           account.ReceivedDate);
    }
}