using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Enums;

namespace AccountingOffice.Domain.Core.Aggregates;

public class AccountPayable : Account<Guid>
{

    #region Propriedades
    public DateTime? PaymentDate { get; private set; }

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
    private AccountPayable(Guid id,
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
    private static DomainResult ValidatePayableParameters(DateTime? paymentDate, AccountStatus status)
    {
        List<string> errors = new();
        if (paymentDate.HasValue && status != AccountStatus.Paid)
            errors.Add("A data de pagamento só pode ser se o status for pago .");

        if (paymentDate.HasValue && paymentDate.Value > DateTime.Now)
            errors.Add("Payment date cannot be in the future.");

        if (errors.Any())
            return DomainResult.Failure(string.Join("|", errors));

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
    public static DomainResult<AccountPayable> Create(Guid id,
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

        validationResult = ValidatePayableParameters(paymentDate, status);
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
    

    #endregion

}
