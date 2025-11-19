using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.Events;

/// <summary>
/// Evento publicado quando uma conta a pagar é criada
/// </summary>
public sealed record AccountPayableCreatedEvent(
    Guid AccountId,
    Guid TenantId,
    decimal Amount,
    DateTime IssueDate,
    DateTime DueDate,
    string Description) : IEvent;

/// <summary>
/// Evento publicado quando uma conta a receber é criada
/// </summary>
public sealed record AccountReceivableCreatedEvent(
    Guid AccountId,
    Guid TenantId,
    decimal Amount,
    DateTime IssueDate,
    DateTime DueDate,
    string Description) : IEvent;

/// <summary>
/// Evento publicado quando uma parcela é paga
/// </summary>
public sealed record InstallmentPaidEvent(
    Guid AccountId,
    Guid TenantId,
    int InstallmentNumber,
    decimal Amount,
    DateTime PaymentDate) : IEvent;

/// <summary>
/// Evento publicado quando uma conta é atualizada
/// </summary>
public sealed record AccountUpdatedEvent(
    Guid AccountId,
    Guid TenantId,
    decimal NewAmount,
    DateTime UpdatedAt) : IEvent;
