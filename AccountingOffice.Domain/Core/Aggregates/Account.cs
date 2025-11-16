using AccountingOffice.Domain.Core.Enums;
using AccountingOffice.Domain.Core.Interfaces;

namespace AccountingOffice.Domain.Core.Aggregates;

public abstract class Account<TId> : IMultiTenantEntity<TId>
{

    protected Account(TId id,
                   TId tenantId,
                   string description,
                   decimal ammount,
                   DateTime issueDate,
                   DateTime dueDate,
                   AccountStatus status,
                   Person<TId> relatedParty)
    {
        if (EqualityComparer<TId>.Default.Equals(tenantId, default!))
            throw new ArgumentException("TenantId é obrigatório.", nameof(tenantId));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("A descrição não pode ser nula ou vazia.", nameof(description));

        if (ammount <= 0)
            throw new ArgumentException("O valor deve ser maior que zero.", nameof(ammount));

        if (issueDate > dueDate)
            throw new ArgumentException("Data de emissão não pode ser maior que data de vencimento.", nameof(issueDate));

        if (relatedParty == null)
            throw new ArgumentNullException(nameof(relatedParty));

        // Validação: RelatedParty deve pertencer ao mesmo tenant
        // Como Person sempre usa Guid para TenantId e Account<TId> quando TId=Guid também usa Guid
        if (tenantId is Guid accountTenantId && !accountTenantId.Equals(relatedParty.TenantId))
            throw new ArgumentException("A pessoa relacionada deve pertencer à mesma empresa.", nameof(relatedParty));

        Id = id;
        TenantId = tenantId;
        Description = description;
        Ammount = ammount;
        DueDate = dueDate;
        IssueDate = issueDate;
        Status = status;
        RelatedParty = relatedParty;
    }

    public TId Id { get; protected set; }
    public TId TenantId { get; protected set; }
    public string Description { get; protected set; }
    public decimal Ammount { get; protected set; }
    public DateTime IssueDate { get; protected set; } // Data de emissão
    public DateTime DueDate { get; protected set; } // Data de vencimento
    public AccountStatus Status { get; protected set; }
    public Person<TId> RelatedParty { get; protected set; }
}