using AccountingOffice.Domain.Core.Enums;
using AccountingOffice.Domain.Core.ValueObjects;

namespace AccountingOffice.Domain.Core.Aggregates;

public class AccountPayable : Account<Guid>
{

    public AccountPayable(
        Guid id,
        Guid tenantId,
        string description,
        decimal ammount,
        DateTime issueDate,
        DateTime dueDate,
        AccountStatus status,
        Person<Guid> supplier,
        PaymentMethod payMethod,
        DateTime? paymentDate = null)
        : base(id, tenantId, description, ammount, issueDate, dueDate, status, supplier)
    {
        if (paymentDate.HasValue && status != AccountStatus.Paid)
            throw new ArgumentException("Payment date can only be set when status is Paid.", nameof(paymentDate));

        if (paymentDate.HasValue && paymentDate.Value > DateTime.Now)
            throw new ArgumentException("Payment date cannot be in the future.", nameof(paymentDate));

        PayMethod = payMethod;
        PaymentDate = paymentDate;
    }

    public PaymentMethod PayMethod { get; private set; }
    public DateTime? PaymentDate { get; private set; }

    private  List<Installment> _installments = new();

    public IReadOnlyCollection<Installment> Installments => _installments.AsReadOnly();

    public void AddInstallment(Installment installment)
    {
        if (installment == null)
            throw new ArgumentNullException(nameof(installment));

        if (_installments.Any(i => i.InstallmentNumber == installment.InstallmentNumber))
            throw new ArgumentException($"Installment with number {installment.InstallmentNumber} already exists.", nameof(installment));

        if (installment.DueDate < IssueDate)
            throw new ArgumentException("Installment due date cannot be before account issue date.", nameof(installment));

        if (installment.DueDate > DueDate)
            throw new ArgumentException("Installment due date cannot be after account due date.", nameof(installment));

        _installments.Add(installment);
    }
}
