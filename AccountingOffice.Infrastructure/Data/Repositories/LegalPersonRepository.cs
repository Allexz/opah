using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Domain.Core.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace AccountingOffice.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório para pessoas jurídicas.
/// </summary>
public class LegalPersonRepository : ILegalPersonRepository
{
    private readonly AccountingOfficeDbContext _dbContext;

    public LegalPersonRepository(AccountingOfficeDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Adiciona uma nova pessoa jurídica ao banco de dados.
    /// </summary>
    /// <param name="legalPerson">A pessoa jurídica a ser adicionada.</param>
    /// <returns>A pessoa jurídica adicionada.</returns>
    public async Task<LegalPerson> CreateAsync(LegalPerson legalPerson)
    {
        _dbContext.Persons.Add(legalPerson);
        await _dbContext.SaveChangesAsync();
        return legalPerson;
    }

    /// <summary>
    /// Atualiza uma pessoa jurídica existente no banco de dados.
    /// </summary>
    /// <param name="legalPerson">A pessoa jurídica a ser atualizada.</param>
    /// <returns>A pessoa jurídica atualizada.</returns>
    public async Task<LegalPerson> UpdateAsync(LegalPerson legalPerson)
    {
        _dbContext.Persons.Update(legalPerson);
        await _dbContext.SaveChangesAsync();
        return legalPerson;
    }

    /// <summary>
    /// Remove uma pessoa jurídica do banco de dados pelo seu ID.
    /// </summary>
    /// <param name="id">O ID da pessoa jurídica a ser removida.</param>
    /// <returns>True se a remoção foi bem-sucedida, false se não encontrada.</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _dbContext.Persons.OfType<LegalPerson>().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return false;

        _dbContext.Persons.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
