using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Enums;
using AccountingOffice.Domain.Core.ValueObjects;
using Bogus;
using FakeItEasy;

namespace AccountOffice.Tests.Domain.Account;

public class AccountPayableTests
{
    private readonly Faker _faker;
    private readonly Guid _validTenantId;
    private readonly Guid _validId;
    private readonly Person<Guid> _validSupplier;

    public AccountPayableTests()
    {
        _faker = new Faker("pt_BR");
        _validTenantId = Guid.NewGuid();
        _validId = Guid.NewGuid();

        var supplierResult = LegalPerson.Create(
            Guid.NewGuid(),
            _validTenantId,
            _faker.Company.CompanyName(),
            "11.222.333/0001-81",
            PersonType.Company,
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber(),
            _faker.Company.CompanyName());
        _validSupplier = supplierResult.Value;
    }

    private AccountPayable CreateValidAccountPayable(
        AccountStatus? status = null,
        DateTime? paymentDate = null)
    {
        var result = AccountPayable.Create(
            _validId,
            _validTenantId,
            _faker.Lorem.Sentence(),
            _faker.Finance.Amount(100, 10000),
            DateTime.UtcNow.AddDays(-10),
            DateTime.UtcNow.AddDays(20),
            status ?? AccountStatus.Pending,
            _validSupplier,
            _faker.PickRandom<PaymentMethod>(),
            paymentDate);

        return result.Value;
    }

    [Fact]
    public void AccountPayable_Should_Create_Successfully_With_Valid_Parameters()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);
        var payMethod = _faker.PickRandom<PaymentMethod>();

        // Act
        var result = AccountPayable.Create(
            _validId,
            _validTenantId,
            description,
            amount,
            issueDate,
            dueDate,
            AccountStatus.Pending,
            _validSupplier,
            payMethod);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(payMethod, result.Value.PayMethod);
        Assert.Null(result.Value.PaymentDate);
        Assert.Empty(result.Value.Installments);
    }

    [Fact]
    public void AccountPayable_Should_Create_With_PaymentDate_When_Status_Is_Paid()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);
        var paymentDate = DateTime.UtcNow.AddDays(-5);
        var payMethod = _faker.PickRandom<PaymentMethod>();

        // Act
        var result = AccountPayable.Create(
            _validId,
            _validTenantId,
            description,
            amount,
            issueDate,
            dueDate,
            AccountStatus.Paid,
            _validSupplier,
            payMethod,
            paymentDate);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(AccountStatus.Paid, result.Value.Status);
        Assert.NotNull(result.Value.PaymentDate);
        Assert.Equal(paymentDate.Date, result.Value.PaymentDate.Value.Date);
    }

    [Fact]
    public void AccountPayable_Should_Fail_When_PaymentDate_Is_Set_But_Status_Is_Not_Paid()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);
        var paymentDate = DateTime.UtcNow.AddDays(-5);

        // Act
        var result = AccountPayable.Create(
            _validId,
            _validTenantId,
            description,
            amount,
            issueDate,
            dueDate,
            AccountStatus.Pending,
            _validSupplier,
            PaymentMethod.BankTransfer,
            paymentDate);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("data de pagamento", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AccountPayable_Should_Fail_When_PaymentDate_Is_In_The_Future()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);
        var futurePaymentDate = DateTime.UtcNow.AddDays(1);

        // Act
        var result = AccountPayable.Create(
            _validId,
            _validTenantId,
            description,
            amount,
            issueDate,
            dueDate,
            AccountStatus.Paid,
            _validSupplier,
            PaymentMethod.BankTransfer,
            futurePaymentDate);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("futuro", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AccountPayable_Should_Add_Installment_Successfully()
    {
        // Arrange
        var account = CreateValidAccountPayable();
        var installmentResult = Installment.Create(
            1,
            500m,
            DateTime.UtcNow.AddDays(15),
            AccountStatus.Pending,EntryType.Debit);

        // Act
        var result = account.AddInstallment(installmentResult.Value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(account.Installments);
        Assert.Equal(1, account.Installments.First().InstallmentNumber);
        Assert.Equal(500m, account.Installments.First().Amount);
    }

    [Fact]
    public void AccountPayable_Should_Add_Multiple_Installments()
    {
        // Arrange
        AccountPayable account = CreateValidAccountPayable();
        var installment1 = Installment.Create(1, 500m, DateTime.UtcNow.AddDays(15), AccountStatus.Pending, EntryType.Debit).Value;
        var installment2 = Installment.Create(2, 500m, DateTime.UtcNow.AddDays(10), AccountStatus.Pending, EntryType.Debit).Value;

        // Act
        account.AddInstallment(installment1);
        account.AddInstallment(installment2);

        // Assert
        Assert.Equal(2, account.Installments.Count);
    }

    [Fact]
    public void AccountPayable_Should_Fail_When_Adding_Null_Installment()
    {
        // Arrange
        var account = CreateValidAccountPayable();

        // Act
        var result = account.AddInstallment(null!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("nula", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AccountPayable_Should_Fail_When_Adding_Duplicate_Installment_Number()
    {
        // Arrange
        var account = CreateValidAccountPayable();
        var installment1 = Installment.Create(1, 500m, DateTime.UtcNow.AddDays(15), AccountStatus.Pending, EntryType.Debit).Value;
        var installment2 = Installment.Create(1, 600m, DateTime.UtcNow.AddDays(16), AccountStatus.Pending, EntryType.Debit).Value;

        account.AddInstallment(installment1);

        // Act
        var result = account.AddInstallment(installment2);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("já existe", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AccountPayable_Should_Fail_When_Installment_DueDate_Is_Before_IssueDate()
    {
        // Arrange
        var account = CreateValidAccountPayable();
        var installment = Installment.Create(
            1,
            500m,
            account.IssueDate.AddDays(-1), // Antes da data de emissão
            AccountStatus.Pending,
            EntryType.Debit).Value;

        // Act
        var result = account.AddInstallment(installment);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("emissão", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AccountPayable_Should_Fail_When_Installment_DueDate_Is_After_DueDate()
    {
        // Arrange
        var account = CreateValidAccountPayable();
        var installment = Installment.Create(
            1,
            500m,
            account.DueDate.AddDays(1), // Depois da data de vencimento
            AccountStatus.Pending,
            EntryType.Debit).Value;

        // Act
        var result = account.AddInstallment(installment);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("vencimento", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AccountPayable_Should_Allow_Installment_DueDate_Equals_IssueDate()
    {
        // Arrange
        var account = CreateValidAccountPayable();
        var installment = Installment.Create(
            1,
            500m,
            account.IssueDate,
            AccountStatus.Pending,
            EntryType.Debit).Value;

        // Act
        var result = account.AddInstallment(installment);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AccountPayable_Should_Allow_Installment_DueDate_Equals_DueDate()
    {
        // Arrange
        var account = CreateValidAccountPayable();
        var installment = Installment.Create(
            1,
            500m,
            account.DueDate,
            AccountStatus.Pending, EntryType.Debit).Value;

        // Act
        var result = account.AddInstallment(installment);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(PaymentMethod.Cash)]
    [InlineData(PaymentMethod.CreditCard)]
    [InlineData(PaymentMethod.DebitCard)]
    [InlineData(PaymentMethod.BankTransfer)]
    [InlineData(PaymentMethod.Check)]
    [InlineData(PaymentMethod.Pix)]
    public void AccountPayable_Should_Accept_All_Payment_Methods(PaymentMethod paymentMethod)
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);

        // Act
        var result = AccountPayable.Create(
            _validId,
            _validTenantId,
            description,
            amount,
            issueDate,
            dueDate,
            AccountStatus.Pending,
            _validSupplier,
            paymentMethod);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(paymentMethod, result.Value.PayMethod);
    }
}

