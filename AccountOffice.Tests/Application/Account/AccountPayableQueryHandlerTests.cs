using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.UseCases.AccountPay.Queries;
using AccountingOffice.Application.UseCases.AccountPay.Queries.Result;
using AccountingOffice.Application.UseCases.AccountPay.QueryHandler;
using AccountingOffice.Application.UseCases.Installm.Commands;
using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Enums;
using AccountingOffice.Domain.Core.ValueObjects;
using Bogus;
using FakeItEasy;
using System.Linq;

namespace AccountOffice.Tests.Application.Account;

public class AccountPayableQueryHandlerTests
{
    private readonly Faker _faker;
    private readonly Guid _tenantId;
    private readonly Guid _accountId;
    private readonly Person<Guid> _supplier;
    private readonly IAccountPayableQuery _accountPayableQuery;
    private readonly AccountPayableQueryHandler _handler;

    public AccountPayableQueryHandlerTests()
    {
        _faker = new Faker("pt_BR");
        _tenantId = Guid.NewGuid();
        _accountId = Guid.NewGuid();

        var supplierResult = LegalPerson.Create(
            Guid.NewGuid(),
            _tenantId,
            _faker.Company.CompanyName(),
            "11.222.333/0001-81",
            PersonType.Company,
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber(),
            _faker.Company.CompanyName());
        _supplier = supplierResult.Value;

        _accountPayableQuery = A.Fake<IAccountPayableQuery>();
        _handler = new AccountPayableQueryHandler(_accountPayableQuery);
    }

    private AccountPayable CreateValidAccountPayable()
    {
        var result = AccountPayable.Create(
            _accountId,
            _tenantId,
            _faker.Lorem.Sentence(),
            _faker.Finance.Amount(100, 10000),
            DateTime.UtcNow.AddDays(-10),
            DateTime.UtcNow.AddDays(20),
            AccountStatus.Pending,
            _supplier,
            _faker.PickRandom<PaymentMethod>());
        return result.Value;
    }

    [Fact]
    public async Task Handle_GetAccountPayByTenantIdQuery_Should_Succeed_When_Accounts_Exist()
    {
        // Arrange
        var query = new GetAccountPayByTenantIdQuery(_tenantId, 1, 10);
        var accounts = new List<AccountPayable> { CreateValidAccountPayable() };

        A.CallTo(() => _accountPayableQuery.GetByTenantIdAsync(_tenantId, 1, 10, A<CancellationToken>.Ignored)).Returns(accounts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal(_accountId, result.Value.First().id);
    }

    [Fact]
    public async Task Handle_GetAccountPayByTenantIdQuery_Should_Fail_When_No_Accounts_Found()
    {
        // Arrange
        var query = new GetAccountPayByTenantIdQuery(_tenantId, 1, 10);

        A.CallTo(() => _accountPayableQuery.GetByTenantIdAsync(_tenantId, 1, 10, A<CancellationToken>.Ignored)).Returns(new List<AccountPayable>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Não foram localizada", result.Error);
    }

    

    [Fact]
    public async Task Handle_GetAccountPayByRelatedPartQuery_Should_Fail_When_No_Accounts_Found()
    {
        // Arrange
        var relatedPartId = Guid.NewGuid();
        var query = new GetAccountPayByRelatedPartQuery(relatedPartId, _tenantId, 1, 10);

        A.CallTo(() => _accountPayableQuery.GetByRelatedPartyId(_tenantId, relatedPartId, 1, 10, A<CancellationToken>.Ignored)).Returns(new List<AccountPayable>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("parceiro informado", result.Error);
    }

    [Fact]
    public async Task Handle_GetAccountPayByIssueDateQuery_Should_Succeed_When_Accounts_Exist()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var query = new GetAccountPayByIssueDateQuery(startDate, endDate, _tenantId, 1, 10);
        var accounts = new List<AccountPayable> { CreateValidAccountPayable() };

        A.CallTo(() => _accountPayableQuery.GetByIssueDateAsync(_tenantId, startDate, endDate, 1, 10, A<CancellationToken>.Ignored)).Returns(accounts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }

    [Fact]
    public async Task Handle_GetAccountPayByIssueDateQuery_Should_Fail_When_No_Accounts_Found()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var query = new GetAccountPayByIssueDateQuery(startDate, endDate, _tenantId, 1, 10);

        A.CallTo(() => _accountPayableQuery.GetByIssueDateAsync(_tenantId, startDate, endDate, 1, 10, A<CancellationToken>.Ignored)).Returns(new List<AccountPayable>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("período informado", result.Error);
    }

    [Fact]
    public async Task Handle_GetAccountPayByIdQuery_Should_Succeed_When_Account_Exists()
    {
        // Arrange
        var query = new GetAccountPayByIdQuery(_accountId, _tenantId);
        var account = CreateValidAccountPayable();

        A.CallTo(() => _accountPayableQuery.GetByIdAsync(_accountId, _tenantId, A<CancellationToken>.Ignored)).Returns(account);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_accountId, result.Value.id);
    }

    [Fact]
    public async Task Handle_GetAccountPayByIdQuery_Should_Fail_When_Account_Not_Found()
    {
        // Arrange
        var query = new GetAccountPayByIdQuery(_accountId, _tenantId);

        A.CallTo(() => _accountPayableQuery.GetByIdAsync(_accountId, _tenantId, A<CancellationToken>.Ignored)).Returns((AccountPayable?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("não localizada", result.Error);
    }
}
