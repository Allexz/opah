using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Enums;

namespace AccountingOffice.Domain.Core.Aggregates;

public class AccountReceivable : Account<Guid>
{
    #region Propriedades
   
    public string InvoiceNumber { get; private set; } 
    public DateTime? ReceivedDate { get; private set; }
    
    #endregion

    #region Construtores
    private AccountReceivable(
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
        PayMethod = payMethod;
        InvoiceNumber = invoiceNumber;
        ReceivedDate = receivedDate;
    }

    #endregion

    #region Validação
    private static DomainResult ValidateReceivableParameters(DateTime? receivedDate, AccountStatus status, string invoiceNumber )
    {
        List<string> errors = new();
 
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            errors.Add("Identificador da parcela não pode ser vazio.");

        if (receivedDate.HasValue && status != AccountStatus.Received)
            errors.Add("Data de recebimento só pode ser preenchida junto com status de recebida.");

        if (receivedDate.HasValue && receivedDate.Value > DateTime.Now)
            errors.Add("Data de recebimento não pode ser marcada para o futuro.");

        if (errors.Any())
            return DomainResult.Failure(string.Join("|", errors));

        return DomainResult.Success();
    }
    #endregion

    #region Alterações de estado
    public static DomainResult<AccountReceivable>  Create(
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
        
        DomainResult? baseValidationResult = ValidateAccountParameters(tenantId,
                                                         description,
                                                         ammount,
                                                         issueDate,
                                                         dueDate,
                                                         status,
                                                         customer);
        if (baseValidationResult.IsFailure)
            return DomainResult<AccountReceivable>.Failure(baseValidationResult.Error);

        baseValidationResult = ValidateReceivableParameters(receivedDate, status, invoiceNumber);

        if (baseValidationResult.IsFailure)
            return DomainResult<AccountReceivable>.Failure(baseValidationResult.Error);

        return DomainResult<AccountReceivable>.Success ( new AccountReceivable(
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

    #endregion


}
