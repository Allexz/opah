using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Domain.Core.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace AccountingOffice.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório para pessoas físicas.
/// </summary>
public class IndividualPersonRepository : IIndividualPersonRepository
{
    private readonly AccountingOfficeDbContext _dbContext;

    public IndividualPersonRepository(AccountingOfficeDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Adiciona uma nova pessoa física ao banco de dados.
    /// </summary>
    /// <param name="individualPerson">A pessoa física a ser adicionada.</param>
    /// <returns>A pessoa física adicionada.</returns>
    public async Task<IndividualPerson> CreateAsync(IndividualPerson individualPerson)
    {
        _dbContext.Persons.Add(individualPerson);
        await _dbContext.SaveChangesAsync();
        return individualPerson;
    }

    /// <summary>
    /// Atualiza uma pessoa física existente no banco de dados.
    /// </summary>
    /// <param name="individualPerson">A pessoa física a ser atualizada.</param>
    /// <returns>A pessoa física atualizada.</returns>
    public async Task<IndividualPerson> UpdateAsync(IndividualPerson individualPerson)
    {
        _dbContext.Persons.Update(individualPerson);
        await _dbContext.SaveChangesAsync();
        return individualPerson;
    }

    /// <summary>
    /// Remove uma pessoa física do banco de dados pelo seu ID.
    /// </summary>
    /// <param name="id">O ID da pessoa física a ser removida.</param>
    /// <returns>True se a remoção foi bem-sucedida, false se não encontrada.</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _dbContext.Persons.OfType<IndividualPerson>().FirstOrDefaultAsync(x=> x.Id == id);
        if (entity == null) return false;

        _dbContext.Persons.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
