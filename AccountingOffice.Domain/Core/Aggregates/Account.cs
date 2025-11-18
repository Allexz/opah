using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Enums;
using AccountingOffice.Domain.Core.Interfaces;
using AccountingOffice.Domain.Core.ValueObjects;

namespace AccountingOffice.Domain.Core.Aggregates;

public abstract class Account<TId> : IMultiTenantEntity<TId>
{
    #region Propriedades
    public TId Id { get; protected set; }
    public TId TenantId { get; protected set; }
    public string Description { get; protected set; }
    public decimal Ammount { get; protected set; }
    public DateTime IssueDate { get; protected set; } // Data de emissão
    public DateTime DueDate { get; protected set; } // Data de vencimento
    public AccountStatus Status { get; protected set; }
    public Person<TId> RelatedParty { get; protected set; }

    protected List<Installment> _installments = new();
    public IReadOnlyCollection<Installment> Installments => _installments.AsReadOnly();

    public PaymentMethod PayMethod { get; protected set; }

    #endregion
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

    protected static DomainResult ValidateAccountParameters(TId tenantId,
                                                      string description,
                                                      decimal ammount,
                                                      DateTime issueDate,
                                                      DateTime dueDate,
                                                      AccountStatus Status,
                                                      Person<TId> relatedParty)
    {
        List<string> errors = new();
        if (EqualityComparer<TId>.Default.Equals(tenantId, default!))
            errors.Add("TenantId é obrigatório.");

        if (!Enum.IsDefined(typeof(AccountStatus), Status))
            errors.Add("Status da conta inválido.");

        if (string.IsNullOrWhiteSpace(description))
            errors.Add("A descrição não pode ser nula ou vazia." );

        if (ammount <= 0)
            errors.Add("O valor deve ser maior que zero.");

        if (issueDate > dueDate)
            errors.Add("Data de emissão não pode ser maior que data de vencimento.");

        if (relatedParty == null)
            errors.Add("Pessoa não pode ser nula");

        // Validação: RelatedParty deve pertencer ao mesmo tenant
        // Como Person sempre usa Guid para TenantId, precisamos comparar corretamente
        // Quando TId = Guid, tenantId já é Guid, então podemos fazer a comparação direta
        if (typeof(TId) == typeof(Guid))
        {
            var accountTenantId = (Guid)(object)tenantId!;
            if (relatedParty != null && !accountTenantId.Equals(relatedParty!.TenantId))
                errors.Add("A pessoa relacionada deve pertencer à mesma empresa.");
        }

        if (errors.Any())
            return DomainResult.Failure(string.Join("|", errors));

        return DomainResult.Success();
    }

    public DomainResult ChangeDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription))
            return DomainResult.Failure("A descrição não pode ser nula ou vazia.");

        if (newDescription.Length > 500)
            return DomainResult.Failure("A descrição não pode ter mais de 500 caracteres.");

        Description = newDescription;
        return DomainResult.Success();
    }
    public DomainResult ChangeStatus(AccountStatus newStatus)
    {
        if (!Enum.IsDefined(typeof(AccountStatus), newStatus))
            return DomainResult.Failure("Status da conta inválido.");

        Status = newStatus;
        return DomainResult.Success();
    }
    public DomainResult ChangeDueDate(DateTime newDueDate)
    {
        if (newDueDate < IssueDate)
            return DomainResult.Failure("Data de vencimento não pode ser anterior a data de emissão.");

        DueDate = newDueDate;
        return DomainResult.Success();
    }
    public DomainResult AddInstallment(Installment installment)
    {
        if (installment == null)
            return DomainResult.Failure("Parcela não pode ser nula");

        if (_installments.Any(i => i.InstallmentNumber == installment.InstallmentNumber))
            return DomainResult.Failure($"Parcela com o identificador {installment.InstallmentNumber} já existe.");

        if (installment.DueDate < IssueDate)
            return DomainResult.Failure("Data de vencimento da parcela não pode ser anterior a data de emissão da conta.");

        if (installment.DueDate > DueDate)
            return DomainResult.Failure("Data de vencimento da parcela não pode ser após a data de vencimento da conta.");

        _installments.Add(installment);

        return DomainResult.Success();
    }
    public DomainResult ChangePaymentMethod(PaymentMethod newPaymentMethod)
    {
        if (!Enum.IsDefined(typeof(PaymentMethod), newPaymentMethod))
            return DomainResult.Failure("Método de pagamento inválido.");

        if (_installments.Any(x => x.Status == AccountStatus.Paid))
            return DomainResult.Failure("Não é possível alterar o método de pagamento quando existem parcelas associadas à conta que já foram pagas.");

        PayMethod = newPaymentMethod;
        return DomainResult.Success();
    }


}