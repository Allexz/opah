using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Enums;

namespace AccountOffice.Tests.Domain.Person;

public class IndividualPersonTests
{
    private readonly Guid _validTenantId = Guid.NewGuid();
    private readonly Guid _validId = Guid.NewGuid();
    private const string ValidName = "João Silva";
    private const string ValidEmail = "joao@example.com";
    private const string ValidPhone = "(11) 98765-4321";
    private const string ValidCpf = "111.444.777-35";

    [Fact]
    public void Create_Should_Succeed_With_Valid_Parameters()
    {
        // Arrange & Act
        var result = IndividualPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCpf,
            PersonType.Individual,
            ValidEmail,
            ValidPhone,
            MaritalStatus.Single);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.IsType<IndividualPerson>(result.Value);
        Assert.Equal(MaritalStatus.Single, result.Value.MaritalStatus);
    }

    [Fact]
    public void Create_Should_Fail_When_Cpf_Is_Invalid()
    {
        // Arrange
        const string invalidCpf = "123.456.789-00";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => IndividualPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            invalidCpf,
            PersonType.Individual,
            ValidEmail,
            ValidPhone,
            MaritalStatus.Single));
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("123456789")]
    [InlineData("123.456.789-0")]
    [InlineData("000.000.000-00")]
    [InlineData("111.111.111-11")]
    public void Create_Should_Fail_With_Invalid_Cpf_Formats(string invalidCpf)
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => IndividualPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            invalidCpf,
            PersonType.Individual,
            ValidEmail,
            ValidPhone,
            MaritalStatus.Single));
    }

    [Fact]
    public void Create_Should_Accept_Cpf_With_Or_Without_Formatting()
    {
        // Arrange
        const string cpfWithFormatting = "111.444.777-35";
        const string cpfWithoutFormatting = "11144477735";

        // Act
        var result1 = IndividualPerson.Create(
            Guid.NewGuid(),
            _validTenantId,
            ValidName,
            cpfWithFormatting,
            PersonType.Individual,
            ValidEmail,
            ValidPhone,
            MaritalStatus.Single);

        var result2 = IndividualPerson.Create(
            Guid.NewGuid(),
            _validTenantId,
            ValidName,
            cpfWithoutFormatting,
            PersonType.Individual,
            ValidEmail,
            ValidPhone,
            MaritalStatus.Single);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.Equal("111.444.777-35", result1.Value.Document);
        Assert.Equal("11144477735", result2.Value.Document);
    }

    [Theory]
    [InlineData(MaritalStatus.Single)]
    [InlineData(MaritalStatus.Married)]
    [InlineData(MaritalStatus.Divorced)]
    [InlineData(MaritalStatus.Widowed)]
    [InlineData(MaritalStatus.Separated)]
    public void Create_Should_Succeed_With_All_Valid_MaritalStatus_Values(MaritalStatus maritalStatus)
    {
        // Arrange & Act
        var result = IndividualPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCpf,
            PersonType.Individual,
            ValidEmail,
            ValidPhone,
            maritalStatus);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(maritalStatus, result.Value.MaritalStatus);
    }

    [Fact]
    public void ChangeMaritalStatus_Should_Update_MaritalStatus_When_Valid()
    {
        // Arrange
        var person = IndividualPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCpf,
            PersonType.Individual,
            ValidEmail,
            ValidPhone,
            MaritalStatus.Single).Value;

        // Act
        var result = person.ChangeMaritalStatus(MaritalStatus.Married);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(MaritalStatus.Married, person.MaritalStatus);
    }

    [Fact]
    public void ChangeMaritalStatus_Should_Fail_When_Status_Is_Invalid()
    {
        // Arrange
        var person = IndividualPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCpf,
            PersonType.Individual,
            ValidEmail,
            ValidPhone,
            MaritalStatus.Single).Value;

        // Act
        var invalidStatus = (MaritalStatus)999;
        var result = person.ChangeMaritalStatus(invalidStatus);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Estado civil inválido", result.Error);
    }

    [Fact]
    public void IndividualPerson_Should_Have_Correct_PersonType()
    {
        // Arrange & Act
        var result = IndividualPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCpf,
            PersonType.Individual,
            ValidEmail,
            ValidPhone,
            MaritalStatus.Single);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(PersonType.Individual, result.Value.Type);
    }

    [Fact]
    public void IndividualPerson_Should_Store_Document_As_Provided()
    {
        // Arrange
        const string cpfWithFormatting = "111.444.777-35";

        // Act
        var result = IndividualPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            cpfWithFormatting,
            PersonType.Individual,
            ValidEmail,
            ValidPhone,
            MaritalStatus.Single);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("111.444.777-35", result.Value.Document);
    }

    [Fact]
    public void IndividualPerson_Active_Property_Can_Be_Set()
    {
        // Arrange & Act
        var result = IndividualPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCpf,
            PersonType.Individual,
            ValidEmail,
            ValidPhone,
            MaritalStatus.Single);

        // Assert
        Assert.True(result.IsSuccess);
        // Active é uma propriedade que pode ser definida, não tem valor padrão garantido
        result.Value.Active = true;
        Assert.True(result.Value.Active);
    }

    [Fact]
    public void IndividualPerson_Can_Be_Deactivated()
    {
        // Arrange
        var person = IndividualPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCpf,
            PersonType.Individual,
            ValidEmail,
            ValidPhone,
            MaritalStatus.Single).Value;

        // Act
        person.Active = false;

        // Assert
        Assert.False(person.Active);
    }

    [Fact]
    public void IndividualPerson_Can_Be_Reactivated()
    {
        // Arrange
        var person = IndividualPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCpf,
            PersonType.Individual,
            ValidEmail,
            ValidPhone,
            MaritalStatus.Single).Value;

        person.Active = false;

        // Act
        person.Active = true;

        // Assert
        Assert.True(person.Active);
    }
}

