using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.AccountReceiv.Commands;

public sealed record UpdateAccountReceivableCommand : ICommand<Result<bool>>
{
    /// <summary>
    /// Identificador da conta a receber.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identificador da conta a receber.
    /// </summary>
    public Guid TenantId { get; init; }

    /// <summary>
    /// Descrição da conta a receber.
    /// </summary>
    public string Description { get; init; }


    /// <summary>
    /// Data de vencimento da conta a receber.
    /// </summary>
    public DateTime DueDate { get; init; }

    /// <summary>
    /// Forma de pagamento da conta a receber.
    /// </summary>
    public int PayMethod { get; init; }

    /// <summary>
    /// Construtor do comando de atualização da conta a receber.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="description"></param>
    /// <param name="amount"></param>
    /// <param name="dueDate"></param>
    /// <param name="payMethod"></param>
    /// <param name="invoiceNumber"></param>
    public UpdateAccountReceivableCommand(
        Guid id,
        Guid tenantId,
        string description = "",
        DateTime dueDate = default,
        int payMethod = 0)
    {
        Id = id;
        TenantId = tenantId;
        Description = description;
        DueDate = dueDate == default ? DateTime.UtcNow : dueDate;
        PayMethod = payMethod;
    }

    public bool HasDescription => !string.IsNullOrWhiteSpace(Description);
    public bool HasDueDate => DueDate != default;
    public bool HasPayMethod => PayMethod != 0;
    
}
