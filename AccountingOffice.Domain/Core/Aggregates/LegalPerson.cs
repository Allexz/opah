using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Enums;
using AccountingOffice.Domain.Core.Validators;

namespace AccountingOffice.Domain.Core.Aggregates;

/// <summary>
/// Representa uma pessoa jurídica, uma empresa ou organização.
/// </summary>
public class LegalPerson : Person<Guid>
{

    #region Propriedades
    /// <summary>
    /// Razão social da pessoa jurídica.
    /// </summary>
    public string LegalName { get; private set; }

    #endregion

    #region Construtor
    /// <summary>
    /// Construtor privado para criação de uma pessoa jurídica.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tenantId"></param>
    /// <param name="name"></param>
    /// <param name="document"></param>
    /// <param name="type"></param>
    /// <param name="email"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="legalName"></param>
    /// <exception cref="ArgumentException"></exception>
    private LegalPerson(
       Guid id,
       Guid tenantId,
       string name,
       string document,
       PersonType type,
       string email,
       string phoneNumber,
       string legalName)
       : base(id, tenantId, name, document, type, email, phoneNumber)
    {
        LegalName = legalName;
    }
    #endregion

    #region Validação
    /// <summary>
    /// Validação dos parâmetros específicos para criação de uma pessoa jurídica.
    /// </summary>
    /// <param name="legalName"></param>
    /// <param name="document"></param>
    /// <returns></returns>
    private static DomainResult ValidateCreationParameters(string legalName, string document)
    {
        if (string.IsNullOrWhiteSpace(legalName))
            return DomainResult.Failure("Razão social não pode ser nula ou vazia.");

        if (!LegalPersonDocValidator.IsCnpj(document))
            return DomainResult.Failure("CNPJ inválido.");

        return DomainResult.Success();
    }
    #endregion

    #region Alterações de estado

    /// <summary>
    /// Método de fábrica para criar uma nova pessoa jurídica.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tenantId"></param>
    /// <param name="name"></param>
    /// <param name="document"></param>
    /// <param name="type"></param>
    /// <param name="email"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="legalName"></param>
    /// <returns></returns>
    public static DomainResult<LegalPerson> Create(
        Guid id,
        Guid tenantId,
        string name,
        string document,
        PersonType type,
        string email,
        string phoneNumber,
        string legalName)
    {
        LegalPerson legalPerson = new LegalPerson(
            id,
            tenantId,
            name,
            document,
            type,
            email,
            phoneNumber,
            legalName);

        DomainResult? baseValidation = ValidatePersonParameters(tenantId, name, document, email, phoneNumber);
        if (baseValidation.IsFailure)
            return DomainResult<LegalPerson>.Failure(baseValidation.Error);

        DomainResult? validationResult = ValidateCreationParameters(legalName, document);
        if (validationResult.IsFailure)
            return DomainResult<LegalPerson>.Failure(validationResult.Error);

        return DomainResult<LegalPerson>.Success(legalPerson);

    }

    /// <summary>
    /// Altera a razão social da pessoa jurídica.
    /// </summary>
    /// <param name="newLegalName"></param>
    /// <returns></returns>
    public DomainResult ChangeLegalName(string newLegalName)
    {
        if (string.IsNullOrWhiteSpace(newLegalName))
            return DomainResult.Failure("Razão social não pode ser nula ou vazia.");

        LegalName = newLegalName;
        return DomainResult.Success();
    }

    #endregion
}
