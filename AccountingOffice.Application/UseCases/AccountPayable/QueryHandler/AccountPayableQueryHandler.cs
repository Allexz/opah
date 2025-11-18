using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.UseCases.AccountPay.Queries;
using AccountingOffice.Application.UseCases.AccountPay.Queries.Result;
using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Application.UseCases.AccountPay.QueryHandler;

public class AccountPayableQueryHandler:
    IQueryHandler<GetAccountPayByTenantIdQuery,Result<IEnumerable<AccountPayableResult>>>
{
    private readonly IAccountPayableQuery _accountPayableQuery;

    public AccountPayableQueryHandler(IAccountPayableQuery accountPayableQuery)
    {
        _accountPayableQuery = accountPayableQuery ?? throw new ArgumentNullException(nameof(accountPayableQuery));
    }

    public async Task<Result<IEnumerable<AccountPayableResult>>> Handle(GetAccountPayByTenantIdQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<AccountPayable> accounts = await _accountPayableQuery.GetByTenantIdAsync(request.TenantId, request.PageNum,request.PageSize, cancellationToken);
        if (!accounts.Any())
        {
            return Result<IEnumerable<AccountPayableResult>>.Failure("Não foram localizadas contas a pagar");
        }
        return Result<IEnumerable<AccountPayableResult>>.Success(accounts.Select(MapToAccountPayableResult));
    }

    public async Task<Result<IEnumerable<AccountPayableResult>>> Handle(GetAccountPayByRelatedPartQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<AccountPayable> accounts = await _accountPayableQuery.GetByRelatedPartyId(request.TenantId,request.TenantId,request.PageNum,request.PageSize, cancellationToken);
        if (!accounts.Any())
        {
            return Result<IEnumerable<AccountPayableResult>>.Failure("Não foram localizadas contas a pagar para o parceiro informado");
        }
        return Result<IEnumerable<AccountPayableResult>>.Success(accounts.Select(MapToAccountPayableResult));
    }

    public async Task<Result<IEnumerable<AccountPayableResult>>> Handle(GetAccountPayByIssueDateQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<AccountPayable> accounts = await _accountPayableQuery.GetByIssueDateAsync(request.TenantId, request.StartDate,request.EndDate,request.PageNum,request.PageSize, cancellationToken);
        if (!accounts.Any())
        {
            return Result<IEnumerable<AccountPayableResult>>.Failure("Não foram localizadas contas a pagar para o período informado");
        }
        return Result<IEnumerable<AccountPayableResult>>.Success(accounts.Select(MapToAccountPayableResult));
    }

    public async Task<Result<AccountPayableResult?>> Handle(GetAccountPayByIdQuery request, CancellationToken cancellationToken)
    {
        AccountPayable? account = await _accountPayableQuery.GetByIdAsync(request.Id, request.TenantId,cancellationToken);
        if (account is null)
        {
            return Result<AccountPayableResult?>.Failure("Conta a pagar não localizada");
        }
        return Result<AccountPayableResult?>.Success( MapToAccountPayableResult(account));
    }

    private static AccountPayableResult MapToAccountPayableResult(AccountPayable account)
    {
        return new AccountPayableResult(account.Id,
                                        account.TenantId,
                                        account.Description,
                                        account.Ammount,
                                        account.IssueDate,
                                        account.DueDate,
                                        account.Status,
                                        account.RelatedParty,
                                        account.PayMethod,
                                        account.PaymentDate);
    }
}
