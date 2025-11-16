using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Enums;
using AccountingOffice.Domain.Core.ValueObjects;

namespace AccountingOffice.Domain.Core.Aggregates;

public class AccountReceivable : Account<Guid>
{
    #region Propriedades
    public PaymentMethod PayMethod { get; private set; }
    public string InvoiceNumber { get; private set; } = string.Empty;
    public DateTime? ReceivedDate { get; private set; }

    private List<Installment> _installments = new();
    public IReadOnlyCollection<Installment> Installments => _installments.AsReadOnly();
    #endregion

    #region Construtores
    public AccountReceivable(
        Guid id,
        Guid tenantId,
        string description,
        decimal ammount,
        DateTime dueDate,
        DateTime issueDate,
        AccountStatus status,
        Person<Guid> customer,
        PaymentMethod payMethod,
        string invoiceNumber,
        DateTime? receivedDate = null)
        : base(id, tenantId, description, ammount, issueDate, dueDate, status, customer)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new ArgumentException("Identificador da parcela não pode ser vazio.", nameof(invoiceNumber));

        if (receivedDate.HasValue && status != AccountStatus.Received)
            throw new ArgumentException("Data de recebimento só pode ser preenchida junto com status de recebida.", nameof(receivedDate));

        if (receivedDate.HasValue && receivedDate.Value > DateTime.Now)
            throw new ArgumentException("Data de recebimento não pode ser marcada para o futuro.", nameof(receivedDate));

        PayMethod = payMethod;
        InvoiceNumber = invoiceNumber;
        ReceivedDate = receivedDate;
    }

    #endregion

    #region Validação
    private static Result ValidateCreationParameters(DateTime? receivedDate, AccountStatus status)
    {
        if (receivedDate.HasValue && status != AccountStatus.Received)
            return Result.Failure("Data de recebimento só pode ser preenchida junto com status de recebida.");
        if (receivedDate.HasValue && receivedDate.Value > DateTime.Now)
            return Result.Failure("Data de recebimento não pode ser marcada para o futuro.");

        return Result.Success();
    }
    #endregion

    #region Alterações de estado
    public static Result<AccountReceivable>  Create(
        Guid id,
        Guid tenantId,
        string description,
        decimal ammount,
        DateTime dueDate,
        DateTime issueDate,
        AccountStatus status,
        Person<Guid> customer,
        PaymentMethod payMethod,
        string invoiceNumber,
        DateTime? receivedDate = null)
    {
        
        Result? baseValidationResult = ValidateAccountParameters(tenantId,
                                                         description,
                                                         ammount,
                                                         issueDate,
                                                         dueDate,
                                                         status,
                                                         customer);
        if (baseValidationResult.IsFailure)
            return Result<AccountReceivable>.Failure(baseValidationResult.Error);

        baseValidationResult = ValidateCreationParameters(receivedDate, status);

        if (baseValidationResult.IsFailure)
            return Result<AccountReceivable>.Failure(baseValidationResult.Error);

        return Result<AccountReceivable>.Success ( new AccountReceivable(
            id,
            tenantId,
            description,
            ammount,
            dueDate,
            issueDate,
            status,
            customer,
            payMethod,
            invoiceNumber,
            receivedDate));
    }
    public void AddInstallment(Installment installment)
    {
        if (installment == null)
            throw new ArgumentNullException(nameof(installment));

        if (_installments.Any(i => i.InstallmentNumber == installment.InstallmentNumber))
            throw new ArgumentException($"Parcela com identificador {installment.InstallmentNumber} já existe.", nameof(installment));

        if (installment.DueDate < IssueDate)
            throw new ArgumentException("Data de vencimento não pode ser maior que data de emissão.", nameof(installment));

        if (installment.DueDate > DueDate)
            throw new ArgumentException("Vencimento da parcela não pode ser maior que o vencimento da conta principal", nameof(installment));

        _installments.Add(installment);
    }

    #endregion


}
