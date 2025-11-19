using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Application.Interfaces.Repositories;

/// <summary>
/// Interface para o repositório de pessoas físicas.
/// </summary>
public interface IIndividualPersonRepository
{
    /// <summary>
    /// Adiciona uma nova pessoa física ao DB
    /// </summary>
    /// <param name="individualPerson"></param>
    /// <returns></returns>
    Task<IndividualPerson> CreateAsync(IndividualPerson individualPerson);

    /// <summary>
    /// Atualiza uma pessoa física existente no DB
    /// </summary>
    /// <param name="individualPerson"></param>
    /// <returns></returns>
    Task<IndividualPerson> UpdateAsync(IndividualPerson individualPerson);

    /// <summary>
    /// Exclui uma pessoa física do DB pelo seu Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Guid id);
}
