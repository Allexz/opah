using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Application.Interfaces.Repositories;

/// <summary>
/// Interface para o repositório de empresas/tenants.
/// </summary>
public interface ICompanyRepository
{
    /// <summary>
    /// Adiciona uma nova empresa ao DB
    /// </summary>
    /// <param name="company"></param>
    /// <returns></returns>
    Task<Company> CreateAsync(Company company);

    /// <summary>
    /// Atualiza uma empresa existente no DB
    /// </summary>
    /// <param name="company"></param>
    /// <returns></returns>
    Task<Company> UpdateAsync(Company company);

    /// <summary>
    /// Exclui uma empresa do DB pelo seu Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Guid id);


}

