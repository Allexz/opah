namespace AccountingOffice.Domain.Core.Interfaces;

/// <summary>
/// Interface que identifica entidades que pertencem a uma empresa/tenant específica
/// </summary>
public interface IMultiTenantEntity<TId>
{
    /// <summary>
    /// Identificador da empresa/tenant à qual a entidade pertence
    /// </summary>
    TId TenantId { get; }
}

