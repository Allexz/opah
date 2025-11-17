using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Enums;
using AccountingOffice.Domain.Core.ValueObjects;

namespace AccountingOffice.Domain.Core.Aggregates;

public class AccountPayable : Account<Guid>
{

    #region Propriedades
    public PaymentMethod PayMethod { get; private set; }
    public DateTime? PaymentDate { get; private set; }

    private List<Installment> _installments = new();
    public IReadOnlyCollection<Installment> Installments => _installments.AsReadOnly();

    #endregion

    #region Construtores
    /// <summary>
    /// Construtor privado para criação de uma conta a pagar.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tenantId"></param>
    /// <param name="description"></param>
    /// <param name="ammount"></param>
    /// <param name="issueDate"></param>
    /// <param name="dueDate"></param>
    /// <param name="status"></param>
    /// <param name="supplier"></param>
    /// <param name="payMethod"></param>
    /// <param name="paymentDate"></param>
    private AccountPayable(
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
        PayMethod = payMethod;
        PaymentDate = paymentDate;
    }
    #endregion

    #region Validação
    /// <summary>
    /// Validação dos parâmetros específicos para criação de uma conta a pagar.
    /// </summary>
    /// <param name="paymentDate"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    private static DomainResult ValidateCreationParameters(DateTime? paymentDate, AccountStatus status)
    {
        if (paymentDate.HasValue && status != AccountStatus.Paid)
            return DomainResult.Failure("A data de pagamento só pode ser se o status for pago .");

        if (paymentDate.HasValue && paymentDate.Value > DateTime.Now)
            return DomainResult.Failure("Payment date cannot be in the future.");

        return DomainResult.Success();
    }

    #endregion

    #region Alterações de estado

    /// <summary>
    /// Fábrica para criação de uma conta a pagar.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tenantId"></param>
    /// <param name="description"></param>
    /// <param name="ammount"></param>
    /// <param name="issueDate"></param>
    /// <param name="dueDate"></param>
    /// <param name="status"></param>
    /// <param name="supplier"></param>
    /// <param name="payMethod"></param>
    /// <param name="paymentDate"></param>
    /// <returns></returns>
    public static DomainResult<AccountPayable> Create(
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
    {
        DomainResult? validationResult = ValidateAccountParameters(tenantId,
                                                             description,
                                                             ammount,
                                                             issueDate,
                                                             dueDate,
                                                             status,
                                                             supplier);
        if (!validationResult.IsSuccess)
            return DomainResult<AccountPayable>.Failure(validationResult.Error);

        validationResult = ValidateCreationParameters(paymentDate, status);
        if (!validationResult.IsSuccess)
            return DomainResult<AccountPayable>.Failure(validationResult.Error);

        AccountPayable accountPayable = new AccountPayable(
            id,
            tenantId,
            description,
            ammount,
            issueDate,
            dueDate,
            status,
            supplier,
            payMethod,
            paymentDate);

        return DomainResult<AccountPayable>.Success(accountPayable);
    }
    public DomainResult AddInstallment(Installment installment)
    {
        if (installment == null)
           return DomainResult.Failure("Parcela não pode ser nula");

        if (_installments.Any(i => i.InstallmentNumber == installment.InstallmentNumber))
            return DomainResult.Failure($"Parcela com o identificador {installment.InstallmentNumber} já existe.");

        if (installment.DueDate < IssueDate)
            return DomainResult.Failure("Data de vencimento da parcela não pode ser anterior a data de emissão da conta.");

        if (installment.DueDate > DueDate)
            return DomainResult.Failure("Data de vencimento da parcela não pode ser após a data de vencimento da conta.");

        _installments.Add(installment);

        return DomainResult.Success();
    }

    #endregion

}
