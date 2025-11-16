using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Enums;
using Bogus;
using FakeItEasy;

namespace AccountOffice.Tests.Domain.Account;

public class AccountTests
{
    private readonly Faker _faker;
    private readonly Guid _validTenantId;
    private readonly Guid _validId;
    private readonly Person<Guid> _validSupplier;
    private readonly Person<Guid> _validCustomer;

    public AccountTests()
    {
        _faker = new Faker("pt_BR");
        _validTenantId = Guid.NewGuid();
        _validId = Guid.NewGuid();

        // Criar pessoas válidas usando FakeItEasy
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

    [Fact]
    public void Account_Should_Have_All_Properties_Set_When_Created_As_AccountPayable()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);
        var status = AccountStatus.Pending;
        var payMethod = _faker.PickRandom<PaymentMethod>();

        // Act
        var result = AccountPayable.Create(
            _validId,
            _validTenantId,
            description,
            amount,
            issueDate,
            dueDate,
            status,
            _validSupplier,
            payMethod);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_validId, result.Value.Id);
        Assert.Equal(_validTenantId, result.Value.TenantId);
        Assert.Equal(description, result.Value.Description);
        Assert.Equal(amount, result.Value.Ammount);
        Assert.Equal(issueDate, result.Value.IssueDate);
        Assert.Equal(dueDate, result.Value.DueDate);
        Assert.Equal(status, result.Value.Status);
        Assert.Equal(_validSupplier, result.Value.RelatedParty);
        Assert.Equal(payMethod, result.Value.PayMethod);
    }

    [Fact]
    public void Account_Should_Have_All_Properties_Set_When_Created_As_AccountReceivable()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);
        var status = AccountStatus.Pending;
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
            status,
            _validCustomer,
            payMethod,
            invoiceNumber);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_validId, result.Value.Id);
        Assert.Equal(_validTenantId, result.Value.TenantId);
        Assert.Equal(description, result.Value.Description);
        Assert.Equal(amount, result.Value.Ammount);
        Assert.Equal(issueDate, result.Value.IssueDate);
        Assert.Equal(dueDate, result.Value.DueDate);
        Assert.Equal(status, result.Value.Status);
        Assert.Equal(_validCustomer, result.Value.RelatedParty);
        Assert.Equal(payMethod, result.Value.PayMethod);
        Assert.Equal(invoiceNumber, result.Value.InvoiceNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Account_Should_Fail_When_Description_Is_Empty_Or_Null(string? invalidDescription)
    {
        // Arrange
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);

        // Act
        var result = AccountPayable.Create(
            _validId,
            _validTenantId,
            invalidDescription!,
            amount,
            issueDate,
            dueDate,
            AccountStatus.Pending,
            _validSupplier,
            PaymentMethod.BankTransfer);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("descrição", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Account_Should_Fail_When_Amount_Is_Zero()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);

        // Act
        var result = AccountPayable.Create(
            _validId,
            _validTenantId,
            description,
            0,
            issueDate,
            dueDate,
            AccountStatus.Pending,
            _validSupplier,
            PaymentMethod.BankTransfer);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("valor", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Account_Should_Fail_When_Amount_Is_Negative()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);

        // Act
        var result = AccountPayable.Create(
            _validId,
            _validTenantId,
            description,
            -100,
            issueDate,
            dueDate,
            AccountStatus.Pending,
            _validSupplier,
            PaymentMethod.BankTransfer);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("valor", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Account_Should_Fail_When_IssueDate_Is_Greater_Than_DueDate()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(20);
        var dueDate = DateTime.UtcNow.AddDays(10);

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
            PaymentMethod.BankTransfer);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("emissão", result.Error, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("vencimento", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Account_Should_Fail_When_TenantId_Is_Empty()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);

        // Act
        var result = AccountPayable.Create(
            _validId,
            Guid.Empty,
            description,
            amount,
            issueDate,
            dueDate,
            AccountStatus.Pending,
            _validSupplier,
            PaymentMethod.BankTransfer);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("TenantId", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Account_Should_Fail_When_RelatedParty_Is_Null()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var issueDate = DateTime.UtcNow.AddDays(-10);
        var dueDate = DateTime.UtcNow.AddDays(20);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AccountPayable.Create(
            _validId,
            _validTenantId,
            description,
            amount,
            issueDate,
            dueDate,
            AccountStatus.Pending,
            null!,
            PaymentMethod.BankTransfer));
    }

    [Fact]
    public void Account_Should_Fail_When_RelatedParty_TenantId_Differs_From_Account_TenantId()
    {
        // Arrange
        var differentTenantId = Guid.NewGuid();
        var supplierResult = LegalPerson.Create(
            Guid.NewGuid(),
            differentTenantId, // TenantId diferente do _validTenantId
            _faker.Company.CompanyName(),
            "11.222.333/0001-81",
            PersonType.Company,
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber(),
            _faker.Company.CompanyName());
        
        Assert.True(supplierResult.IsSuccess); // Garantir que a pessoa foi criada com sucesso
        var supplierWithDifferentTenant = supplierResult.Value;

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
            supplierWithDifferentTenant,
            PaymentMethod.BankTransfer);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("mesma empresa", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Account_Should_Allow_IssueDate_Equals_DueDate()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var amount = _faker.Finance.Amount(100, 10000);
        var sameDate = DateTime.UtcNow.AddDays(10);

        // Act
        var result = AccountPayable.Create(
            _validId,
            _validTenantId,
            description,
            amount,
            sameDate,
            sameDate,
            AccountStatus.Pending,
            _validSupplier,
            PaymentMethod.BankTransfer);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(sameDate, result.Value.IssueDate);
        Assert.Equal(sameDate, result.Value.DueDate);
    }
}

