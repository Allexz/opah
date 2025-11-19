using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório para empresas/tenants.
/// </summary>
public class CompanyRepository : ICompanyRepository
{
    private readonly AccountingOfficeDbContext _dbContext;

    public CompanyRepository(AccountingOfficeDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Adiciona uma nova empresa ao banco de dados.
    /// </summary>
    /// <param name="company">A empresa a ser adicionada.</param>
    /// <returns>A empresa adicionada.</returns>
    public async Task<Company> CreateAsync(Company company)
    {
        _dbContext.Companies.Add(company);
        await _dbContext.SaveChangesAsync();
        return company;
    }

    /// <summary>
    /// Atualiza uma empresa existente no banco de dados.
    /// </summary>
    /// <param name="company">A empresa a ser atualizada.</param>
    /// <returns>A empresa atualizada.</returns>
    public async Task<Company> UpdateAsync(Company company)
    {
        _dbContext.Companies.Update(company);
        await _dbContext.SaveChangesAsync();
        return company;
    }

    /// <summary>
    /// Remove uma empresa do banco de dados pelo seu ID.
    /// </summary>
    /// <param name="id">O ID da empresa a ser removida.</param>
    /// <returns>True se a remoção foi bem-sucedida, false se não encontrada.</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _dbContext.Companies.FindAsync(id);
        if (entity == null) return false;

        _dbContext.Companies.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
