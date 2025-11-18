using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Enums;
using AccountingOffice.Domain.Core.ValueObjects;
using Bogus;
using FakeItEasy;

namespace AccountOffice.Tests.Domain.Account;

public class AccountReceivableTests
{
    private readonly Faker _faker;
    private readonly Guid _validTenantId;
    private readonly Guid _validId;
    private readonly Person<Guid> _validCustomer;

    public AccountReceivableTests()
    {
        _faker = new Faker("pt_BR");
        _validTenantId = Guid.NewGuid();
        _validId = Guid.NewGuid();

        var customerResult = IndividualPerson.Create(
            Guid.NewGuid(),
            _validTenantId,
            _faker.Person.FullName,
            "111.444.777-35",
            PersonType.Individual,
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber(),
            MaritalStatus.Single);
        _validCustomer = customerResult.Value;
    }

    private AccountReceivable CreateValidAccountReceivable(
        AccountStatus? status = null,
        DateTime? receivedDate = null)
    {
        var result = AccountReceivable.Create(
            _validId,
            _validTenantId,
            _faker.Lorem.Sentence(),
            _faker.Finance.Amount(100, 10000),
            DateTime.UtcNow.AddDays(20),
            DateTime.UtcNow.AddDays(-10),
            status ?? AccountStatus.Pending,
            _validCustomer,
            _faker.PickRandom<PaymentMethod>(),
            _faker.Random.AlphaNumeric(10),
            receivedDate);

        return result.Value;
    }

    [Fact]
    public void AccountReceivable_Should_Create_Successfully_With_Valid_Parameters()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);
        var payMethod = _faker.PickRandom<PaymentMethod>();
        var invoiceNumber = _faker.Random.AlphaNumeric(10);

        // Act
        var result = AccountReceivable.Create(
            _validId,
            _validTenantId,
            description,
            amount,
            dueDate,
            issueDate,
            AccountStatus.Pending,
            _validCustomer,
            payMethod,
            invoiceNumber);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(payMethod, result.Value.PayMethod);
        Assert.Equal(invoiceNumber, result.Value.InvoiceNumber);
        Assert.Null(result.Value.ReceivedDate);
        Assert.Empty(result.Value.Installments);
    }

    [Fact]
    public void AccountReceivable_Should_Create_With_ReceivedDate_When_Status_Is_Received()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);
        var receivedDate = DateTime.UtcNow.AddDays(-5);
        var invoiceNumber = _faker.Random.AlphaNumeric(10);

        // Act
        var result = AccountReceivable.Create(
            _validId,
            _validTenantId,
            description,
            amount,
            dueDate,
            issueDate,
            AccountStatus.Received,
            _validCustomer,
            PaymentMethod.Pix,
            invoiceNumber,
            receivedDate);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(AccountStatus.Received, result.Value.Status);
        Assert.NotNull(result.Value.ReceivedDate);
        Assert.Equal(receivedDate.Date, result.Value.ReceivedDate.Value.Date);
    }

    [Fact]
    public void AccountReceivable_Should_Fail_When_InvoiceNumber_Is_Empty()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);

        // Act & Assert
        var domainResult = AccountReceivable.Create(
            _validId,
            _validTenantId,
            description,
            amount,
            dueDate,
            issueDate,
            AccountStatus.Pending,
            _validCustomer,
            PaymentMethod.BankTransfer,
            string.Empty);

        Assert.True(domainResult.IsFailure);
        Assert.Contains("identificador", domainResult.Error, StringComparison.OrdinalIgnoreCase);


    }

    [Fact]
    public void AccountReceivable_Should_Fail_When_InvoiceNumber_Is_Whitespace()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);

        // Act & Assert
        var domainResult = AccountReceivable.Create(
            _validId,
            _validTenantId,
            description,
            amount,
            dueDate,
            issueDate,
            AccountStatus.Pending,
            _validCustomer,
            PaymentMethod.BankTransfer,
            "   ");

        Assert.True(domainResult.IsFailure);
        Assert.Contains("identificador", domainResult.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AccountReceivable_Should_Fail_When_ReceivedDate_Is_Set_But_Status_Is_Not_Received()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);
        var receivedDate = DateTime.UtcNow.AddDays(-5);
        var invoiceNumber = _faker.Random.AlphaNumeric(10);

        // Act
        var result = AccountReceivable.Create(
            _validId,
            _validTenantId,
            description,
            amount,
            dueDate,
            issueDate,
            AccountStatus.Pending,
            _validCustomer,
            PaymentMethod.BankTransfer,
            invoiceNumber,
            receivedDate);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("recebimento", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AccountReceivable_Should_Fail_When_ReceivedDate_Is_In_The_Future()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);
        var futureReceivedDate = DateTime.UtcNow.AddDays(1);
        var invoiceNumber = _faker.Random.AlphaNumeric(10);

        // Act
        var result = AccountReceivable.Create(
            _validId,
            _validTenantId,
            description,
            amount,
            dueDate,
            issueDate,
            AccountStatus.Received,
            _validCustomer,
            PaymentMethod.Pix,
            invoiceNumber,
            futureReceivedDate);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("futuro", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AccountReceivable_Should_Add_Installment_Successfully()
    {
        // Arrange
        var account = CreateValidAccountReceivable();
        var installmentResult = Installment.Create(
            1,
            500m,
            DateTime.UtcNow.AddDays(15),
            AccountStatus.Pending, EntryType.Credit);
        var installment = installmentResult.Value;

        // Act
        account.AddInstallment(installment);

        // Assert
        Assert.Single(account.Installments);
        Assert.Equal(1, account.Installments.First().InstallmentNumber);
        Assert.Equal(500m, account.Installments.First().Amount);
    }

    [Fact]
    public void AccountReceivable_Should_Add_Multiple_Installments()
    {
        // Arrange
        var account = CreateValidAccountReceivable();
        var installment1 = Installment.Create(1, 500m, DateTime.UtcNow.AddDays(15), AccountStatus.Pending, EntryType.Credit).Value;
        var installment2 = Installment.Create(2, 500m, DateTime.UtcNow.AddDays(10), AccountStatus.Pending,EntryType.Credit).Value;

        // Act
        account.AddInstallment(installment1);
        account.AddInstallment(installment2);

        // Assert
        Assert.Equal(2, account.Installments.Count);
    }

    [Fact]
    public void AccountReceivable_Should_Return_Failure_When_Adding_Null_Installment()
    {
        // Arrange
        var account = CreateValidAccountReceivable();

        //Act and Assert
        var domainResult = account.AddInstallment(null!);
        Assert.True(domainResult.IsFailure);
    }

    [Fact]
    public void AccountReceivable_Should_Return_Failure_When_Adding_Duplicate_Installment_Number()
    {
        // Arrange
        var account = CreateValidAccountReceivable();

        //Act and Assert
        account.AddInstallment(Installment.Create(1, 500m, DateTime.UtcNow.AddDays(15), AccountStatus.Pending, EntryType.Credit).Value);
        var domainResult = account.AddInstallment(Installment.Create(1, 600m, DateTime.UtcNow.AddDays(16), AccountStatus.Pending,EntryType.Credit).Value);
        
        Assert.True(domainResult.IsFailure);
        Assert.Contains("já",domainResult.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AccountReceivable_Should_Return_Failure_When_Installment_DueDate_Is_Before_IssueDate()
    {
        // Arrange
        var account = CreateValidAccountReceivable();
        var installment = Installment.Create(
            1,
            500m,
            account.IssueDate.AddDays(-1), // Antes da data de emissão
            AccountStatus.Pending, EntryType.Credit).Value;

        // Act & Assert
        var domainResult = account.AddInstallment(installment);
        Assert.True(domainResult.IsFailure);
        Assert.Contains("vencimento", domainResult.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AccountReceivable_Should_Return_Failure_When_Installment_DueDate_Is_After_DueDate()
    {
        // Arrange
        var account = CreateValidAccountReceivable();
        var installment = Installment.Create(
            1,
            500m,
            account.DueDate.AddDays(1), // Depois da data de vencimento
            AccountStatus.Pending, EntryType.Credit).Value;

        // Act & Assert
        var domainResult = account.AddInstallment(installment);
        Assert.True(domainResult.IsFailure);
        Assert.Contains("vencimento", domainResult.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AccountReceivable_Should_Allow_Installment_DueDate_Equals_IssueDate()
    {
        // Arrange
        var account = CreateValidAccountReceivable();
        var installment = Installment.Create(
            1,
            500m,
            account.IssueDate,
            AccountStatus.Pending, EntryType.Credit).Value;

        // Act
        account.AddInstallment(installment);

        // Assert
        Assert.Single(account.Installments);
    }

    [Fact]
    public void AccountReceivable_Should_Allow_Installment_DueDate_Equals_DueDate()
    {
        // Arrange
        var account = CreateValidAccountReceivable();
        var installment = Installment.Create(
            1,
            500m,
            account.DueDate,
            AccountStatus.Pending, EntryType.Credit).Value;

        // Act
        account.AddInstallment(installment);

        // Assert
        Assert.Single(account.Installments);
    }

    [Theory]
    [InlineData(PaymentMethod.Cash)]
    [InlineData(PaymentMethod.CreditCard)]
    [InlineData(PaymentMethod.DebitCard)]
    [InlineData(PaymentMethod.BankTransfer)]
    [InlineData(PaymentMethod.Check)]
    [InlineData(PaymentMethod.Pix)]
    public void AccountReceivable_Should_Accept_All_Payment_Methods(PaymentMethod paymentMethod)
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);
        var invoiceNumber = _faker.Random.AlphaNumeric(10);

        // Act
        var result = AccountReceivable.Create(
            _validId,
            _validTenantId,
            description,
            amount,
            dueDate,
            issueDate,
            AccountStatus.Pending,
            _validCustomer,
            paymentMethod,
            invoiceNumber);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(paymentMethod, result.Value.PayMethod);
    }

    [Fact]
    public void AccountReceivable_Should_Generate_Unique_Invoice_Numbers()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);
        var invoiceNumber1 = _faker.Random.AlphaNumeric(10);
        var invoiceNumber2 = _faker.Random.AlphaNumeric(10);

        // Act
        var result1 = AccountReceivable.Create(
            Guid.NewGuid(),
            _validTenantId,
            description,
            amount,
            dueDate,
            issueDate,
            AccountStatus.Pending,
            _validCustomer,
            PaymentMethod.Pix,
            invoiceNumber1);

        var result2 = AccountReceivable.Create(
            Guid.NewGuid(),
            _validTenantId,
            description,
            amount,
            dueDate,
            issueDate,
            AccountStatus.Pending,
            _validCustomer,
            PaymentMethod.Pix,
            invoiceNumber2);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.Equal(invoiceNumber1, result1.Value.InvoiceNumber);
        Assert.Equal(invoiceNumber2, result2.Value.InvoiceNumber);
    }
}

