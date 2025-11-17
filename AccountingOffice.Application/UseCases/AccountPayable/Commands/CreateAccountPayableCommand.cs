using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.AccountPay.Commands;

/// <summary>
/// Command para criação de uma conta a pagar (AccountPayable).
/// </summary>
public sealed record CreateAccountPayableCommand : ICommand<Result<Guid>>
{
    public CreateAccountPayableCommand(Guid tenantId,
                                       Guid supplierId,
                                       string description,
                                       decimal ammount,
                                       DateTime dueDate,
                                       int status,
                                       int payMethod,
                                       DateTime? paymentDate = null)
    {
        TenantId = tenantId;
        Description = description;
        Ammount = ammount;
        IssueDate = DateTime.UtcNow;
        DueDate = dueDate;
        Status = status;
        SupplierId = supplierId;
        PayMethod = payMethod;
        PaymentDate = paymentDate ;
    }

    /// <summary>
    /// Identificador do tenant/empresa à qual a conta pertence.
    /// </summary>
    public Guid TenantId { get; init; }

    /// <summary>
    /// Descrição da conta a pagar.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Valor da conta a pagar.
    /// </summary>
    public decimal Ammount { get; init; }

    /// <summary>
    /// Data de emissão da conta.
    /// </summary>
    public DateTime IssueDate { get; init; }

    /// <summary>
    /// Data de vencimento da conta.
    /// </summary>
    public DateTime DueDate { get; init; }

    /// <summary>
    /// Status da conta a pagar.
    /// </summary>
    public int Status { get; init; }

    /// <summary>
    /// Identificador do fornecedor (Person) relacionado à conta.
    /// </summary>
    public Guid SupplierId { get; init; }

    /// <summary>
    /// Método de pagamento.
    /// </summary>
    public int PayMethod { get; init; }

    /// <summary>
    /// Data de pagamento (opcional, só pode ser preenchida se status for Paid).
    /// </summary>
    public DateTime? PaymentDate { get; init; }
}
