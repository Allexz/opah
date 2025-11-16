using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Enums;

namespace AccountingOffice.Domain.Core.ValueObjects;

public class Installment
{

    #region Propriedades
    public int InstallmentNumber { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime DueDate { get; private set; }
    public AccountStatus Status { get; private set; }
    public DateTime? PaymentDate { get; private set; }
    public bool IsPaid => Status == AccountStatus.Paid || Status == AccountStatus.Received;
    public bool IsOverdue => !IsPaid && DueDate < DateTime.Now.Date;

    #endregion

    #region Construtores
    /// <summary>
    /// Construtor privado para inicialização de uma nova parcela.
    /// </summary>
    /// <param name="installmentNumber"></param>
    /// <param name="amount"></param>
    /// <param name="dueDate"></param>
    /// <param name="status"></param>
    /// <param name="paymentDate"></param>
    private Installment(
       int installmentNumber,
       decimal amount,
       DateTime dueDate,
       AccountStatus status,
       DateTime? paymentDate = null)
    {
        InstallmentNumber = installmentNumber;
        Amount = amount;
        DueDate = dueDate;
        Status = status;
        PaymentDate = paymentDate;
    }

    /// <summary>
    /// Inicializador protegido para ORM.
    /// </summary>
    private Installment() { }
    #endregion
    
    #region Validação
    /// <summary>
    /// Validação dos parâmetros para criação de uma nova parcela.
    /// </summary>
    /// <param name="installmentNumber"></param>
    /// <param name="amount"></param>
    /// <param name="dueDate"></param>
    /// <param name="status"></param>
    /// <param name="paymentDate"></param>
    /// <returns></returns>
    private static Result ValidationCreationParameters(int installmentNumber,
        decimal amount,
        DateTime dueDate,
        AccountStatus status,
        DateTime? paymentDate = null)
    {
        if (installmentNumber <= 0)
            return Result.Failure("Installment number must be greater than zero.");
        if (amount <= 0)
            return Result.Failure("Amount must be greater than zero.");
        if (paymentDate.HasValue && paymentDate.Value > DateTime.Now)
            return Result.Failure("Payment date cannot be in the future.");
        return Result.Success();
    }
    #endregion

    #region Alteração de Estado

    /// <summary>
    /// Fábrica para criação de uma nova parcela com validação dos parâmetros.
    /// </summary>
    /// <param name="installmentNumber"></param>
    /// <param name="amount"></param>
    /// <param name="dueDate"></param>
    /// <param name="status"></param>
    /// <param name="paymentDate"></param>
    /// <returns></returns>
    public static Result<Installment> Create(int installmentNumber,
                                             decimal amount,
                                             DateTime dueDate,
                                             AccountStatus status,
                                             DateTime? paymentDate = null)
    {
        Result validationResult = ValidationCreationParameters(installmentNumber,
                                                            amount,
                                                            dueDate,
                                                            status,
                                                            paymentDate);
        if (validationResult.IsFailure)
            return Result<Installment>.Failure(validationResult.Error);

        Installment installment = new Installment(installmentNumber,
                                                  amount,
                                                  dueDate,
                                                  status,
                                                  paymentDate);

        return Result<Installment>.Success(installment);

    }
    /// <summary>
    /// Altera o status da parcela, com validação do pagamento.
    /// </summary>
    /// <param name="newStatus"></param>
    /// <param name="paymentDate"></param>
    /// <returns></returns>
    public Result ChangeStatus(AccountStatus newStatus, DateTime? paymentDate = null)
    {
        if (paymentDate.HasValue && paymentDate.Value > DateTime.Now)
            return Result.Failure("Payment date cannot be in the future.");

        Status = newStatus;
        PaymentDate = paymentDate;

        return Result.Success();
    }   

    #endregion

    #region Implementação de Equals e GetHashCode
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

    #endregion
}

