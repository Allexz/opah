using AccountingOffice.Domain.Core.Enums;

namespace AccountingOffice.Domain.Core.ValueObjects;

public class Installment
{
    public Installment(
        int installmentNumber,
        decimal amount,
        DateTime dueDate,
        AccountStatus status,
        DateTime? paymentDate = null)
    {
        if (installmentNumber <= 0)
            throw new ArgumentException("Installment number must be greater than zero.", nameof(installmentNumber));

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        if (paymentDate.HasValue && paymentDate.Value > DateTime.Now)
            throw new ArgumentException("Payment date cannot be in the future.", nameof(paymentDate));

        InstallmentNumber = installmentNumber;
        Amount = amount;
        DueDate = dueDate;
        Status = status;
        PaymentDate = paymentDate;
    }

    public int InstallmentNumber { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime DueDate { get; private set; }
    public AccountStatus Status { get; private set; }
    public DateTime? PaymentDate { get; private set; }

    public bool IsPaid => Status == AccountStatus.Paid || Status == AccountStatus.Received;
    public bool IsOverdue => !IsPaid && DueDate < DateTime.Now.Date;

    public override bool Equals(object? obj)
    {
        if (obj is not Installment other)
            return false;

        return InstallmentNumber == other.InstallmentNumber &&
               Amount == other.Amount &&
               DueDate == other.DueDate &&
               Status == other.Status &&
               PaymentDate == other.PaymentDate;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(InstallmentNumber, Amount, DueDate, Status, PaymentDate);
    }

    public static bool operator ==(Installment? left, Installment? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Installment? left, Installment? right)
    {
        return !(left == right);
    }
}

