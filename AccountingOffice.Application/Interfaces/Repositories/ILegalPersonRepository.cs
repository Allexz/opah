using AccountingOffice.Domain.Core.Aggregates;

namespace AccountingOffice.Application.Interfaces.Repositories;

public interface ILegalPersonRepository
{
    /// <summary>
    /// Adiciona uma nova pessoa jurídica ao DB
    /// </summary>
    /// <param name="legalPerson"></param>
    /// <returns></returns>
    Task<LegalPerson> CreateAsync(LegalPerson legalPerson);

    /// <summary>
    /// Atualiza uma pessoa jurídica existente no DB
    /// </summary>
    /// <param name="legalPerson"></param>
    /// <returns></returns>
    Task<LegalPerson> UpdateAsync(LegalPerson legalPerson);

    /// <summary>
    /// Exclui uma pessoa jurídica do DB pelo seu Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Guid id);
}
