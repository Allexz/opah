using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Domain.Core.Enums;

namespace AccountingOffice.Application.UseCases.AccountPay.Commands;

/// <summary>
/// Command para atualização de uma conta a pagar (AccountPayable).
/// </summary>
public sealed record UpdateAccountPayableCommand : ICommand<Result<bool>>
{
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
    public AccountStatus Status { get; init; }

    /// <summary>
    /// Método de pagamento.
    /// </summary>
    public PaymentMethod PayMethod { get; init; }

    /// <summary>
    /// Data de pagamento (opcional, só pode ser preenchida se status for Paid).
    /// </summary>
    public DateTime? PaymentDate { get; init; }
}

