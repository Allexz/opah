using AccountingOffice.Domain.Core.Common;
using AccountingOffice.Domain.Core.Enums;
using AccountingOffice.Domain.Core.Validators;

namespace AccountingOffice.Domain.Core.Aggregates;

/// <summary>
/// Representa uma pessoa física, um indivíduo e a informação de seu estado civil.
/// </summary>
public class IndividualPerson : Person<Guid>
{
    #region Propriedades
    public MaritalStatus MaritalStatus { get; private set; }

    #endregion

    #region Construtor
    /// <summary>
    /// Construtor privado para criação de uma pessoa física.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tenantId"></param>
    /// <param name="name"></param>
    /// <param name="document"></param>
    /// <param name="type"></param>
    /// <param name="email"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="maritalStatus"></param>
    /// <exception cref="ArgumentException"></exception>
    private IndividualPerson(
        Guid id,
        Guid tenantId,
        string name,
        string document,
        PersonType type,
        string email,
        string phoneNumber,
        MaritalStatus maritalStatus)
        : base(id, tenantId, name, document, type, email, phoneNumber)
    {
        if (!Enum.IsDefined(typeof(MaritalStatus), maritalStatus))
            throw new ArgumentException("Estado civil inválido.", nameof(maritalStatus));

        if (!IndividualPersonDocValidator.IsCpf(document))
            throw new ArgumentException("CPF inválido.", nameof(document));

        MaritalStatus = maritalStatus;
    }

    #endregion

    #region Validação
    /// <summary>
    /// Valida os parâmetros necessários para a criação de uma pessoa física.
    /// </summary>
    /// <param name="maritalStatus"></param>
    /// <param name="cpfDoc"></param>
    /// <returns></returns>
    private static DomainResult ValidateCreationParameters(MaritalStatus maritalStatus, string cpfDoc)
    {
        if (!Enum.IsDefined(typeof(MaritalStatus), maritalStatus))
            return DomainResult.Failure("Razão social não pode ser nula ou vazia.");

        if (!IndividualPersonDocValidator.IsCpf(cpfDoc))
            return DomainResult.Failure("CPF inválido.");

        return DomainResult.Success();
    }
    #endregion

    #region Alteração de estado
    /// <summary>
    /// Fábrica para criação de uma nova instância de IndividualPerson com validações.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tenantId"></param>
    /// <param name="name"></param>
    /// <param name="document"></param>
    /// <param name="type"></param>
    /// <param name="email"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="maritalStatus"></param>
    /// <returns></returns>
    public static DomainResult<IndividualPerson> Create(
        Guid id,
        Guid tenantId,
        string name,
        string document,
        PersonType type,
        string email,
        string phoneNumber,
        MaritalStatus maritalStatus)
    {
        IndividualPerson individualPerson = new IndividualPerson(
            id,
            tenantId,
            name,

            document,
            type,
            email,
            phoneNumber,
            maritalStatus);

        DomainResult? baseValidation = ValidatePersonParameters(tenantId, name, document, email, phoneNumber);
        if (baseValidation.IsFailure)
            return DomainResult<IndividualPerson>.Failure(baseValidation.Error);

        DomainResult? validationResult = ValidateCreationParameters(maritalStatus, document);
        if (validationResult.IsFailure)
            return DomainResult<IndividualPerson>.Failure(validationResult.Error);

        return DomainResult<IndividualPerson>.Success(individualPerson);

    }

    public DomainResult ChangeMaritalStatus(MaritalStatus newStatus)
    {
        if (!Enum.IsDefined(typeof(MaritalStatus), newStatus))
            return DomainResult.Failure("Estado civil inválido.");

        MaritalStatus = newStatus;
        return DomainResult.Success();
    }

    #endregion
}
