using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Domain.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace AccountingOffice.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório para parcelas de contas.
/// </summary>
public class InstalmentRepository : IInstalmentRepository
{
    private readonly AccountingOfficeDbContext _dbContext;

    public InstalmentRepository(AccountingOfficeDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Adiciona uma nova parcela a uma conta (não implementado completamente, pois parcelas são value objects).
    /// </summary>
    /// <param name="installment">A parcela a ser adicionada.</param>
    /// <returns>True se bem-sucedido.</returns>
    /// <exception cref="NotImplementedException">Operação não implementada.</exception>
    public async Task<bool> CreateAsync(Installment installment)
    {
        // Note: Installment é um value object, não uma entidade separada.
        // Esta operação deve ser feita através do repositório de Account.
        throw new NotImplementedException("Parcelas são gerenciadas através do repositório de contas.");
    }

    /// <summary>
    /// Remove uma parcela de uma conta.
    /// </summary>
    /// <param name="accountId">ID da conta.</param>
    /// <param name="tenantId">ID do tenant.</param>
    /// <param name="installmentNumber">Número da parcela.</param>
    /// <returns>True se a parcela foi removida com sucesso.</returns>
    public async Task<bool> DeleteAsync(Guid accountId, Guid tenantId, int installmentNumber)
    {
        // Buscar a conta no AccountsPayable primeiro
        var accountPayable = await _dbContext.AccountsPayable
            .FirstOrDefaultAsync(a => a.Id == accountId && a.TenantId == tenantId);
        
        if (accountPayable != null && accountPayable.Installments.Any(i => i.InstallmentNumber == installmentNumber))
        {
            // Remover a parcela da coleção
            accountPayable.Installments.ToList().RemoveAll(i => i.InstallmentNumber == installmentNumber);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Se não encontrou na payable, buscar na receivable
        var accountReceivable = await _dbContext.AccountsReceivable
            .FirstOrDefaultAsync(a => a.Id == accountId && a.TenantId == tenantId);

        if (accountReceivable != null && accountReceivable.Installments.Any(i => i.InstallmentNumber == installmentNumber))
        {
            accountReceivable.Installments.ToList().RemoveAll(i => i.InstallmentNumber == installmentNumber);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }
}
