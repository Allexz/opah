using AccountingOffice.Domain.Core.Enums;
using AccountingOffice.Domain.Core.ValueObjects;
using Bogus;

namespace AccountOffice.Tests.Domain.ValueObjects;

public class InstallmentTests
{
    private readonly Faker _faker;

    public InstallmentTests()
    {
        _faker = new Faker("pt_BR");
    }

    #region Create Tests

    [Fact]
    public void Installment_Should_Create_Successfully_With_Valid_Parameters()
    {
        // Arrange
        var installmentNumber = 1;
        var amount = 1000m;
        var dueDate = DateTime.UtcNow.AddDays(30);
        var status = AccountStatus.Pending;

        // Act
        var result = Installment.Create(installmentNumber, amount, dueDate, status);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(installmentNumber, result.Value.InstallmentNumber);
        Assert.Equal(amount, result.Value.Amount);
        Assert.Equal(dueDate.Date, result.Value.DueDate.Date);
        Assert.Equal(status, result.Value.Status);
        Assert.Null(result.Value.PaymentDate);
    }

    [Fact]
    public void Installment_Should_Create_With_PaymentDate_When_Provided()
    {
        // Arrange
        var installmentNumber = 1;
        var amount = 1000m;
        var dueDate = DateTime.UtcNow.AddDays(30);
        var status = AccountStatus.Paid;
        var paymentDate = DateTime.UtcNow.AddDays(-5);

        // Act
        var result = Installment.Create(installmentNumber, amount, dueDate, status, paymentDate);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotNull(result.Value.PaymentDate);
        Assert.Equal(paymentDate.Date, result.Value.PaymentDate.Value.Date);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Installment_Should_Fail_When_InstallmentNumber_Is_Zero_Or_Negative(int invalidNumber)
    {
        // Arrange
        var amount = 1000m;
        var dueDate = DateTime.UtcNow.AddDays(30);
        var status = AccountStatus.Pending;

        // Act
        var result = Installment.Create(invalidNumber, amount, dueDate, status);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("maior", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void Installment_Should_Fail_When_Amount_Is_Zero_Or_Negative(decimal invalidAmount)
    {
        // Arrange
        var installmentNumber = 1;
        var dueDate = DateTime.UtcNow.AddDays(30);
        var status = AccountStatus.Pending;

        // Act
        var result = Installment.Create(installmentNumber, invalidAmount, dueDate, status);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("maior", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Installment_Should_Fail_When_PaymentDate_Is_In_The_Future()
    {
        // Arrange
        var installmentNumber = 1;
        var amount = 1000m;
        var dueDate = DateTime.UtcNow.AddDays(30);
        var status = AccountStatus.Paid;
        var futurePaymentDate = DateTime.UtcNow.AddDays(1);

        // Act
        var result = Installment.Create(installmentNumber, amount, dueDate, status, futurePaymentDate);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("futuro", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region IsPaid Property Tests

    [Theory]
    [InlineData(AccountStatus.Paid, true)]
    [InlineData(AccountStatus.Received, true)]
    [InlineData(AccountStatus.Pending, false)]
    [InlineData(AccountStatus.Cancelled, false)]
    public void Installment_IsPaid_Should_Return_Correct_Value(AccountStatus status, bool expectedIsPaid)
    {
        // Arrange
        var installment = Installment.Create(1, 1000m, DateTime.UtcNow.AddDays(30), status).Value;

        // Act
        var isPaid = installment.IsPaid;

        // Assert
        Assert.Equal(expectedIsPaid, isPaid);
    }

    #endregion

    #region IsOverdue Property Tests

    [Fact]
    public void Installment_IsOverdue_Should_Return_True_When_DueDate_Is_Past_And_Not_Paid()
    {
        // Arrange
        var pastDueDate = DateTime.UtcNow.AddDays(-5);
        var installment = Installment.Create(1, 1000m, pastDueDate, AccountStatus.Pending).Value;

        // Act
        var isOverdue = installment.IsOverdue;

        // Assert
        Assert.True(isOverdue);
    }

    [Fact]
    public void Installment_IsOverdue_Should_Return_False_When_DueDate_Is_Past_But_Paid()
    {
        // Arrange
        var pastDueDate = DateTime.UtcNow.AddDays(-5);
        var paymentDate = DateTime.UtcNow.AddDays(-3);
        var installment = Installment.Create(1, 1000m, pastDueDate, AccountStatus.Paid, paymentDate).Value;

        // Act
        var isOverdue = installment.IsOverdue;

        // Assert
        Assert.False(isOverdue);
    }

    [Fact]
    public void Installment_IsOverdue_Should_Return_False_When_DueDate_Is_Future()
    {
        // Arrange
        var futureDueDate = DateTime.UtcNow.AddDays(30);
        var installment = Installment.Create(1, 1000m, futureDueDate, AccountStatus.Pending).Value;

        // Act
        var isOverdue = installment.IsOverdue;

        // Assert
        Assert.False(isOverdue);
    }

    [Fact]
    public void Installment_IsOverdue_Should_Return_False_When_Status_Is_Received()
    {
        // Arrange
        var pastDueDate = DateTime.UtcNow.AddDays(-5);
        var installment = Installment.Create(1, 1000m, pastDueDate, AccountStatus.Received).Value;

        // Act
        var isOverdue = installment.IsOverdue;

        // Assert
        Assert.False(isOverdue);
    }

    #endregion

    #region ChangeStatus Tests

    [Fact]
    public void Installment_ChangeStatus_Should_Succeed_With_Valid_Parameters()
    {
        // Arrange
        var installment = Installment.Create(1, 1000m, DateTime.UtcNow.AddDays(30), AccountStatus.Pending).Value;
        var newStatus = AccountStatus.Paid;
        var paymentDate = DateTime.UtcNow.AddDays(-1);

        // Act
        var result = installment.ChangeStatus(newStatus, paymentDate);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newStatus, installment.Status);
        Assert.NotNull(installment.PaymentDate);
        Assert.Equal(paymentDate.Date, installment.PaymentDate.Value.Date);
    }

    [Fact]
    public void Installment_ChangeStatus_Should_Succeed_Without_PaymentDate()
    {
        // Arrange
        var installment = Installment.Create(1, 1000m, DateTime.UtcNow.AddDays(30), AccountStatus.Pending).Value;
        var newStatus = AccountStatus.Cancelled;

        // Act
        var result = installment.ChangeStatus(newStatus);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newStatus, installment.Status);
    }

    [Fact]
    public void Installment_ChangeStatus_Should_Fail_When_PaymentDate_Is_In_The_Future()
    {
        // Arrange
        var installment = Installment.Create(1, 1000m, DateTime.UtcNow.AddDays(30), AccountStatus.Pending).Value;
        var newStatus = AccountStatus.Paid;
        var futurePaymentDate = DateTime.UtcNow.AddDays(1);

        // Act
        var result = installment.ChangeStatus(newStatus, futurePaymentDate);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("futuro", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(AccountStatus.Paid)]
    [InlineData(AccountStatus.Received)]
    [InlineData(AccountStatus.Pending)]
    [InlineData(AccountStatus.Cancelled)]
    public void Installment_ChangeStatus_Should_Accept_All_Status_Values(AccountStatus newStatus)
    {
        // Arrange
        var installment = Installment.Create(1, 1000m, DateTime.UtcNow.AddDays(30), AccountStatus.Pending).Value;

        // Act
        var result = installment.ChangeStatus(newStatus);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newStatus, installment.Status);
    }

    #endregion

    #region Equals and GetHashCode Tests

    [Fact]
    public void Installment_Equals_Should_Return_True_For_Same_Values()
    {
        // Arrange
        var installmentNumber = 1;
        var amount = 1000m;
        var dueDate = DateTime.UtcNow.AddDays(30);
        var status = AccountStatus.Pending;
        var paymentDate = DateTime.UtcNow.AddDays(-5);

        var installment1 = Installment.Create(installmentNumber, amount, dueDate, status, paymentDate).Value;
        var installment2 = Installment.Create(installmentNumber, amount, dueDate, status, paymentDate).Value;

        // Act & Assert
        Assert.True(installment1.Equals(installment2));
        Assert.True(installment1 == installment2);
        Assert.False(installment1 != installment2);
    }

    [Fact]
    public void Installment_Equals_Should_Return_False_For_Different_Values()
    {
        // Arrange
        var installment1 = Installment.Create(1, 1000m, DateTime.UtcNow.AddDays(30), AccountStatus.Pending).Value;
        var installment2 = Installment.Create(2, 1000m, DateTime.UtcNow.AddDays(30), AccountStatus.Pending).Value;

        // Act & Assert
        Assert.False(installment1.Equals(installment2));
        Assert.False(installment1 == installment2);
        Assert.True(installment1 != installment2);
    }

    [Fact]
    public void Installment_Equals_Should_Return_False_For_Different_Amounts()
    {
        // Arrange
        var installment1 = Installment.Create(1, 1000m, DateTime.UtcNow.AddDays(30), AccountStatus.Pending).Value;
        var installment2 = Installment.Create(1, 2000m, DateTime.UtcNow.AddDays(30), AccountStatus.Pending).Value;

        // Act & Assert
        Assert.False(installment1.Equals(installment2));
    }

    [Fact]
    public void Installment_Equals_Should_Return_False_For_Different_DueDates()
    {
        // Arrange
        var installment1 = Installment.Create(1, 1000m, DateTime.UtcNow.AddDays(30), AccountStatus.Pending).Value;
        var installment2 = Installment.Create(1, 1000m, DateTime.UtcNow.AddDays(60), AccountStatus.Pending).Value;

        // Act & Assert
        Assert.False(installment1.Equals(installment2));
    }

    [Fact]
    public void Installment_Equals_Should_Return_False_For_Different_Status()
    {
        // Arrange
        var installment1 = Installment.Create(1, 1000m, DateTime.UtcNow.AddDays(30), AccountStatus.Pending).Value;
        var installment2 = Installment.Create(1, 1000m, DateTime.UtcNow.AddDays(30), AccountStatus.Paid).Value;

        // Act & Assert
        Assert.False(installment1.Equals(installment2));
    }

    [Fact]
    public void Installment_Equals_Should_Return_False_For_Different_PaymentDates()
    {
        // Arrange
        var installment1 = Installment.Create(1, 1000m, DateTime.UtcNow.AddDays(30), AccountStatus.Paid, DateTime.UtcNow.AddDays(-5)).Value;
        var installment2 = Installment.Create(1, 1000m, DateTime.UtcNow.AddDays(30), AccountStatus.Paid, DateTime.UtcNow.AddDays(-10)).Value;

        // Act & Assert
        Assert.False(installment1.Equals(installment2));
    }

    [Fact]
    public void Installment_Equals_Should_Return_False_When_Comparing_With_Null()
    {
        // Arrange
        var installment = Installment.Create(1, 1000m, DateTime.UtcNow.AddDays(30), AccountStatus.Pending).Value;

        // Act & Assert
        Assert.False(installment.Equals(null));
        Assert.False(installment == null);
        Assert.True(installment != null);
    }

    [Fact]
    public void Installment_Equals_Should_Return_False_When_Comparing_With_Different_Type()
    {
        // Arrange
        var installment = Installment.Create(1, 1000m, DateTime.UtcNow.AddDays(30), AccountStatus.Pending).Value;
        var differentObject = new object();

        // Act & Assert
        Assert.False(installment.Equals(differentObject));
    }

    [Fact]
    public void Installment_GetHashCode_Should_Return_Same_Value_For_Same_Properties()
    {
        // Arrange
        var installmentNumber = 1;
        var amount = 1000m;
        var dueDate = DateTime.UtcNow.AddDays(30);
        var status = AccountStatus.Pending;
        var paymentDate = DateTime.UtcNow.AddDays(-5);

        var installment1 = Installment.Create(installmentNumber, amount, dueDate, status, paymentDate).Value;
        var installment2 = Installment.Create(installmentNumber, amount, dueDate, status, paymentDate).Value;

        // Act
        var hashCode1 = installment1.GetHashCode();
        var hashCode2 = installment2.GetHashCode();

        // Assert
        Assert.Equal(hashCode1, hashCode2);
    }

    [Fact]
    public void Installment_GetHashCode_Should_Return_Different_Value_For_Different_Properties()
    {
        // Arrange
        var installment1 = Installment.Create(1, 1000m, DateTime.UtcNow.AddDays(30), AccountStatus.Pending).Value;
        var installment2 = Installment.Create(2, 1000m, DateTime.UtcNow.AddDays(30), AccountStatus.Pending).Value;

        // Act
        var hashCode1 = installment1.GetHashCode();
        var hashCode2 = installment2.GetHashCode();

        // Assert
        Assert.NotEqual(hashCode1, hashCode2);
    }

    [Fact]
    public void Installment_Equality_Operators_Should_Handle_Null_Values()
    {
        // Arrange
        Installment? installment1 = null;
        Installment? installment2 = null;
        var installment3 = Installment.Create(1, 1000m, DateTime.UtcNow.AddDays(30), AccountStatus.Pending).Value;

        // Act & Assert
        Assert.True(installment1 == installment2);
        Assert.False(installment1 != installment2);
        Assert.False(installment1 == installment3);
        Assert.True(installment1 != installment3);
        Assert.False(installment3 == installment1);
        Assert.True(installment3 != installment1);
    }

    #endregion
}

