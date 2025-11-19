using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Enums;

namespace AccountOffice.Tests.Domain.Person;

public class LegalPersonTests
{
    private readonly Guid _validTenantId = Guid.NewGuid();
    private readonly Guid _validId = Guid.NewGuid();
    private const string ValidName = "Empresa ABC";
    private const string ValidEmail = "contato@empresaabc.com";
    private const string ValidPhone = "(11) 98765-4321";
    private const string ValidCnpj = "11.222.333/0001-81";
    private const string ValidLegalName = "Empresa ABC LTDA";

    [Fact]
    public void Create_Should_Succeed_With_Valid_Parameters()
    {
        // Arrange & Act
        var result = LegalPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCnpj,
            PersonType.Company,
            ValidEmail,
            ValidPhone,
            ValidLegalName);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.IsType<LegalPerson>(result.Value);
        Assert.Equal(ValidLegalName, result.Value.LegalName);
    }

    [Fact]
    public void Create_Should_Fail_When_Cnpj_Is_Invalid()
    {
        // Arrange
        const string invalidCnpj = "12.345.678/0001-00";

        // Act & Assert
        Assert.True(LegalPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            invalidCnpj,
            PersonType.Company,
            ValidEmail,
            ValidPhone,
            ValidLegalName).IsFailure);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("1234567890123")]
    [InlineData("12.345.678/0001-0")]
    [InlineData("00.000.000/0000-00")]
    [InlineData("11.111.111/1111-11")]
    public void Create_Should_Fail_With_Invalid_Cnpj_Formats(string invalidCnpj)
    {
        // Arrange & Act
        var result = LegalPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            invalidCnpj,
            PersonType.Company,
            ValidEmail,
            ValidPhone,
            ValidLegalName);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Create_Should_Accept_Cnpj_With_Or_Without_Formatting()
    {
        // Arrange
        const string cnpjWithFormatting = "11.222.333/0001-81";
        const string cnpjWithoutFormatting = "11222333000181";

        // Act
        var result1 = LegalPerson.Create(
            Guid.NewGuid(),
            _validTenantId,
            ValidName,
            cnpjWithFormatting,
            PersonType.Company,
            ValidEmail,
            ValidPhone,
            ValidLegalName);

        var result2 = LegalPerson.Create(
            Guid.NewGuid(),
            _validTenantId,
            ValidName,
            cnpjWithoutFormatting,
            PersonType.Company,
            ValidEmail,
            ValidPhone,
            ValidLegalName);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.Equal("11.222.333/0001-81", result1.Value.Document);
        Assert.Equal("11222333000181", result2.Value.Document);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Fail_When_LegalName_Is_Empty_Or_Whitespace(string invalidLegalName)
    {
        // Arrange & Act
        var result = LegalPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCnpj,
            PersonType.Company,
            ValidEmail,
            ValidPhone,
            invalidLegalName);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Razão social não pode ser nula ou vazia", result.Error);
    }

    [Fact]
    public void Create_Should_Fail_When_LegalName_Is_Null()
    {
        // Arrange & Act
        var result = LegalPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCnpj,
            PersonType.Company,
            ValidEmail,
            ValidPhone,
            null!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Razão social não pode ser nula ou vazia", result.Error);
    }

    [Fact]
    public void ChangeLegalName_Should_Update_LegalName_When_Valid()
    {
        // Arrange
        var createResult = LegalPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCnpj,
            PersonType.Company,
            ValidEmail,
            ValidPhone,
            ValidLegalName);
        
        Assert.True(createResult.IsSuccess);
        var person = createResult.Value;

        const string newLegalName = "Nova Razão Social LTDA";

        // Act
        var result = person.ChangeLegalName(newLegalName);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newLegalName, person.LegalName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void ChangeLegalName_Should_Fail_When_LegalName_Is_Empty_Or_Whitespace(string invalidLegalName)
    {
        // Arrange
        var createResult = LegalPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCnpj,
            PersonType.Company,
            ValidEmail,
            ValidPhone,
            ValidLegalName);
        
        Assert.True(createResult.IsSuccess);
        var person = createResult.Value;

        // Act
        var result = person.ChangeLegalName(invalidLegalName);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Razão social não pode ser nula ou vazia", result.Error);
    }

    [Fact]
    public void ChangeLegalName_Should_Fail_When_LegalName_Is_Null()
    {
        // Arrange
        var createResult = LegalPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCnpj,
            PersonType.Company,
            ValidEmail,
            ValidPhone,
            ValidLegalName);
        
        Assert.True(createResult.IsSuccess);
        var person = createResult.Value;

        // Act
        var result = person.ChangeLegalName(null!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Razão social não pode ser nula ou vazia", result.Error);
    }

    [Fact]
    public void LegalPerson_Should_Have_Correct_PersonType()
    {
        // Arrange & Act
        var result = LegalPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCnpj,
            PersonType.Company,
            ValidEmail,
            ValidPhone,
            ValidLegalName);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(PersonType.Company, result.Value.Type);
    }

    [Fact]
    public void LegalPerson_Should_Store_Document_As_Provided()
    {
        // Arrange
        const string cnpjWithFormatting = "11.222.333/0001-81";

        // Act
        var result = LegalPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            cnpjWithFormatting,
            PersonType.Company,
            ValidEmail,
            ValidPhone,
            ValidLegalName);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("11.222.333/0001-81", result.Value.Document);
    }

    [Fact]
    public void LegalPerson_Active_Property_Can_Be_Set()
    {
        // Arrange & Act
        var result = LegalPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCnpj,
            PersonType.Company,
            ValidEmail,
            ValidPhone,
            ValidLegalName);

        // Assert
        Assert.True(result.IsSuccess);
        // Active é uma propriedade que pode ser definida, não tem valor padrão garantido
        result.Value.Active = true;
        Assert.True(result.Value.Active);
    }

    [Fact]
    public void LegalPerson_Can_Be_Deactivated()
    {
        // Arrange
        var createResult = LegalPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCnpj,
            PersonType.Company,
            ValidEmail,
            ValidPhone,
            ValidLegalName);
        
        Assert.True(createResult.IsSuccess);
        var person = createResult.Value;

        // Act
        person.Active = false;

        // Assert
        Assert.False(person.Active);
    }

    [Fact]
    public void LegalPerson_Can_Be_Reactivated()
    {
        // Arrange
        var createResult = LegalPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCnpj,
            PersonType.Company,
            ValidEmail,
            ValidPhone,
            ValidLegalName);
        
        Assert.True(createResult.IsSuccess);
        var person = createResult.Value;

        person.Active = false;

        // Act
        person.Active = true;

        // Assert
        Assert.True(person.Active);
    }

    [Fact]
    public void LegalPerson_Should_Preserve_LegalName_When_Created()
    {
        // Arrange
        const string legalName = "Empresa XYZ Comércio e Serviços LTDA";

        // Act
        var result = LegalPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCnpj,
            PersonType.Company,
            ValidEmail,
            ValidPhone,
            legalName);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(legalName, result.Value.LegalName);
    }

    [Fact]
    public void LegalPerson_Should_Allow_Multiple_Changes_To_LegalName()
    {
        // Arrange
        var createResult = LegalPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCnpj,
            PersonType.Company,
            ValidEmail,
            ValidPhone,
            ValidLegalName);
        
        Assert.True(createResult.IsSuccess);
        var person = createResult.Value;

        // Act
        var result1 = person.ChangeLegalName("Segunda Razão Social");
        var result2 = person.ChangeLegalName("Terceira Razão Social");

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.Equal("Terceira Razão Social", person.LegalName);
    }
}

