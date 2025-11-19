using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Application.UseCases.AccountPay.CommandHandler;
using AccountingOffice.Application.UseCases.AccountPay.Commands;
using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Enums;
using Bogus;
using FakeItEasy;

namespace AccountOffice.Tests.Application.Account;

public class AccountPayableCommandHandlerTests
{
    private readonly Faker _faker;
    private readonly Guid _tenantId;
    private readonly Guid _supplierId;
    private readonly Person<Guid> _supplier;
    private readonly IAccountPayableRepository _accountPayableRepository;
    private readonly IAccountPayableQuery _accountPayableQuery;
    private readonly IPersonQuery _personQuery;
    private readonly AccountPayableCommandHandler _handler;
    private readonly IApplicationBus _applicationBus;

    public AccountPayableCommandHandlerTests()
    {
        _faker = new Faker("pt_BR");
        _tenantId = Guid.NewGuid();
        _supplierId = Guid.NewGuid();

        var supplierResult = LegalPerson.Create(
            _supplierId,
            _tenantId,
            _faker.Company.CompanyName(),
            "11.222.333/0001-81",
            PersonType.Company,
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber(),
            _faker.Company.CompanyName());
        _supplier = supplierResult.Value;

        _accountPayableRepository = A.Fake<IAccountPayableRepository>();
        _accountPayableQuery = A.Fake<IAccountPayableQuery>();
        _personQuery = A.Fake<IPersonQuery>();
        _applicationBus = A.Fake<IApplicationBus>();

        _handler = new AccountPayableCommandHandler(_accountPayableQuery, _accountPayableRepository, _personQuery, _applicationBus);
    }

    [Fact]
    public async Task Handle_CreateAccountPayableCommand_Should_Succeed_When_Supplier_Exists()
    {
        // Arrange
        var command = new CreateAccountPayableCommand(
            _tenantId,
            _supplierId,
            _faker.Lorem.Sentence(),
            _faker.Finance.Amount(100, 10000),
            DateTime.UtcNow.AddDays(20),
            (int)AccountStatus.Pending,
            (int)_faker.PickRandom<PaymentMethod>()
        );

        A.CallTo(() => _personQuery.GetByIdAsync(_supplierId, _tenantId)).Returns(_supplier);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.IsType<Guid>(result.Value);
        A.CallTo(() => _accountPayableRepository.CreateAsync(A<AccountPayable>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_CreateAccountPayableCommand_Should_Fail_When_Supplier_Not_Found()
    {
        // Arrange
        var command = new CreateAccountPayableCommand(
            _tenantId,
            _supplierId,
            _faker.Lorem.Sentence(),
            _faker.Finance.Amount(100, 10000),
            DateTime.UtcNow.AddDays(20),
            (int)AccountStatus.Pending,
            (int)_faker.PickRandom<PaymentMethod>()
        );

        A.CallTo(() => _personQuery.GetByIdAsync(_supplierId, _tenantId)).Returns((Person<Guid>?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Fornecedor não encontrado", result.Error);
        A.CallTo(() => _accountPayableRepository.CreateAsync(A<AccountPayable>.Ignored)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_UpdateAccountPayableCommand_Should_Succeed_When_Account_Exists()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var command = new UpdateAccountPayableCommand(
            _tenantId,
            accountId,
            _faker.Lorem.Sentence(),
            (int)AccountStatus.Paid,
            (int)_faker.PickRandom<PaymentMethod>(),
            DateTime.UtcNow.AddDays(-5)
        );

        var existingAccount = AccountPayable.Create(
            accountId,
            _tenantId,
            _faker.Lorem.Sentence(),
            _faker.Finance.Amount(100, 10000),
            DateTime.UtcNow.AddDays(-10),
            DateTime.UtcNow.AddDays(20),
            AccountStatus.Pending,
            _supplier,
            PaymentMethod.BankTransfer
        ).Value;

        A.CallTo(() => _accountPayableQuery.GetByIdAsync(accountId, _tenantId)).Returns(existingAccount);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        A.CallTo(() => _accountPayableRepository.UpdateAsync(A<AccountPayable>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_UpdateAccountPayableCommand_Should_Fail_When_Account_Not_Found()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var command = new UpdateAccountPayableCommand(
            _tenantId,
            accountId,
            _faker.Lorem.Sentence(),
            (int)AccountStatus.Paid,
            (int)_faker.PickRandom<PaymentMethod>(),
            DateTime.UtcNow.AddDays(-5)
        );

        A.CallTo(() => _accountPayableQuery.GetByIdAsync(accountId, _tenantId)).Returns((AccountPayable?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Conta a pagar não encontrada", result.Error);
        A.CallTo(() => _accountPayableRepository.UpdateAsync(A<AccountPayable>.Ignored)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_DeleteAccountPayableCommand_Should_Succeed_When_Account_Exists()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var command = new DeleteAccountPayableCommand(accountId, _tenantId);

        var existingAccount = AccountPayable.Create(
            accountId,
            _tenantId,
            _faker.Lorem.Sentence(),
            _faker.Finance.Amount(100, 10000),
            DateTime.UtcNow.AddDays(-10),
            DateTime.UtcNow.AddDays(20),
            AccountStatus.Pending,
            _supplier,
            PaymentMethod.BankTransfer
        ).Value;

        A.CallTo(() => _accountPayableQuery.GetByIdAsync(accountId, _tenantId)).Returns(existingAccount);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        A.CallTo(() => _accountPayableRepository.DeleteAsync(accountId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_DeleteAccountPayableCommand_Should_Fail_When_Account_Not_Found()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var command = new DeleteAccountPayableCommand(accountId, _tenantId);

        A.CallTo(() => _accountPayableQuery.GetByIdAsync(accountId, _tenantId)).Returns((AccountPayable?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Conta a pagar não encontrada", result.Error);
        A.CallTo(() => _accountPayableRepository.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
    }
}
