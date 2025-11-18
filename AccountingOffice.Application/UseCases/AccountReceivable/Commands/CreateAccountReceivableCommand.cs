using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.AccountReceiv.Commands;

public sealed class CreateAccountReceivableCommand : ICommand<Result<Guid>>
{
    public Guid TenantId { get; init; }
    public string Description { get; init; }
    public decimal Amount { get; init; }
    public DateTime DueDate { get; init; }
    public DateTime IssueDate { get; init; }
    public Guid CustomerId { get; init; }
    public int PayMethod { get; init; }
    public string InvoiceNumber { get; init; }

    public CreateAccountReceivableCommand(Guid tenantId,
                                          string description,
                                          decimal amount,
                                          DateTime dueDate,
                                          DateTime issueDate,
                                          Guid customerId,
                                          int payMethod,
                                          string invoiceNumber)
    {
        TenantId = tenantId;
        Description = description;
        Amount = amount;
        DueDate = dueDate;
        IssueDate = issueDate;
        CustomerId = customerId;
        PayMethod = payMethod;
        InvoiceNumber = invoiceNumber;
    }
}
