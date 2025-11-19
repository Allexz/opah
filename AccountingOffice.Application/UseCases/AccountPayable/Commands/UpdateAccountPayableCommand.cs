using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Domain.Core.Enums;

namespace AccountingOffice.Application.UseCases.AccountPay.Commands;

/// <summary>
/// Command para atualização de uma conta a pagar (AccountPayable).
/// </summary>
public sealed record UpdateAccountPayableCommand : ICommand<Result<bool>>
{
    public UpdateAccountPayableCommand(Guid tenantId,
                                       Guid id,
                                       string description = "",
                                       int status = 0,
                                       int payMethod = 0,
                                       DateTime? paymentDate = null)
    {
        TenantId = tenantId;
        Id = id;
        Description = description;
        Status = status;
        PayMethod = payMethod;
        PaymentDate = paymentDate;
    }

    public bool HasDescription => !string.IsNullOrWhiteSpace(Description);
    public bool HasStatus => Enum.IsDefined(typeof(AccountStatus), Status);
    public bool HasPayMethod => Enum.IsDefined(typeof(PaymentMethod), PayMethod);
    public bool HasPaymentDate => PaymentDate.HasValue;


    /// <summary>
    /// Identificador do Tenant.
    /// </summary>
    public Guid TenantId { get; init; }

    /// <summary>
    /// Identificador da conta a pagar.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Descrição da conta a pagar.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Status da conta a pagar.
    /// </summary>
    public int Status { get; init; }

    /// <summary>
    /// Método de pagamento.
    /// </summary>
    public int PayMethod { get; init; }

    /// <summary>
    /// Data de pagamento (opcional, só pode ser preenchida se status for Paid).
    /// </summary>
    public DateTime? PaymentDate { get; init; }
}

