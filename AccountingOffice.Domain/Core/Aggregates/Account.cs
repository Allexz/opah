using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Enums;
using AccountingOffice.Domain.Core.Interfaces;

namespace AccountingOffice.Domain.Core.Aggregates;

public abstract class Account<TId> : IMultiTenantEntity<TId>
{
    public TId Id { get; protected set; }
    public TId TenantId { get; protected set; }
    public string Description { get; protected set; }
    public decimal Ammount { get; protected set; }
    public DateTime IssueDate { get; protected set; } // Data de emissão
    public DateTime DueDate { get; protected set; } // Data de vencimento
    public AccountStatus Status { get; protected set; }
    public Person<TId> RelatedParty { get; protected set; }
    protected Account(TId id,
                   TId tenantId,
                   string description,
                   decimal ammount,
                   DateTime issueDate,
                   DateTime dueDate,
                   AccountStatus status,
                   Person<TId> relatedParty)
    {
        Id = id;
        TenantId = tenantId;
        Description = description;
        Ammount = ammount;
        DueDate = dueDate;
        IssueDate = issueDate;
        Status = status;
        RelatedParty = relatedParty;
    }

    protected static Result ValidateAccountParameters(TId tenantId,
                                                      string description,
                                                      decimal ammount,
                                                      DateTime issueDate,
                                                      DateTime dueDate,
                                                      AccountStatus Status,
                                                      Person<TId> relatedParty)
    {
        if (EqualityComparer<TId>.Default.Equals(tenantId, default!))
            return Result.Failure("TenantId é obrigatório.");

        if (!Enum.IsDefined(typeof(AccountStatus), Status))
            return Result.Failure("Status da conta inválido.");

        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure("A descrição não pode ser nula ou vazia." );

        if (ammount <= 0)
            return Result.Failure("O valor deve ser maior que zero.");

        if (issueDate > dueDate)
            return Result.Failure("Data de emissão não pode ser maior que data de vencimento.");

        if (relatedParty == null)
            throw new ArgumentNullException(nameof(relatedParty));

        // Validação: RelatedParty deve pertencer ao mesmo tenant
        // Como Person sempre usa Guid para TenantId, precisamos comparar corretamente
        // Quando TId = Guid, tenantId já é Guid, então podemos fazer a comparação direta
        if (typeof(TId) == typeof(Guid))
        {
            var accountTenantId = (Guid)(object)tenantId!;
            if (!accountTenantId.Equals(relatedParty.TenantId))
                return Result.Failure("A pessoa relacionada deve pertencer à mesma empresa.");
        }

        return Result.Success();
    }


}