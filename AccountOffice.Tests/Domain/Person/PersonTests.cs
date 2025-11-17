using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Enums;

namespace AccountOffice.Tests.Domain.Person;

public class PersonTests
{
    private readonly Guid _validTenantId = Guid.NewGuid();
    private readonly Guid _validId = Guid.NewGuid();
    private const string ValidName = "João Silva";
    private const string ValidEmail = "joao@example.com";
    private const string ValidPhone = "(11) 98765-4321";
    private const string ValidCpf = "111.444.777-35";
    private const string ValidCnpj = "11.222.333/0001-81";

    [Fact]
    public void Person_Should_Have_All_Properties_Set_When_Created_As_IndividualPerson()
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
        Assert.Equal(_validId, result.Value.Id);
        Assert.Equal(_validTenantId, result.Value.TenantId);
        Assert.Equal(ValidName, result.Value.Name);
        Assert.Equal(ValidCpf.Trim(), result.Value.Document);
        Assert.Equal(PersonType.Individual, result.Value.Type);
        Assert.Equal(ValidEmail, result.Value.Email);
        Assert.Equal(ValidPhone, result.Value.Phone);
        Assert.NotEqual(default(DateTime), result.Value.CreatedAt);
    }

    [Fact]
    public void Person_Should_Have_All_Properties_Set_When_Created_As_LegalPerson()
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
            "Razão Social LTDA");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_validId, result.Value.Id);
        Assert.Equal(_validTenantId, result.Value.TenantId);
        Assert.Equal(ValidName, result.Value.Name);
        Assert.Equal(ValidCnpj.Trim(), result.Value.Document);
        Assert.Equal(PersonType.Company, result.Value.Type);
        Assert.Equal(ValidEmail, result.Value.Email);
        Assert.Equal(ValidPhone, result.Value.Phone);
        Assert.NotEqual(default(DateTime), result.Value.CreatedAt);
    }

    [Fact]
    public void Person_Should_Trim_Whitespace_From_Name_Email_And_Phone()
    {
        // Arrange
        const string nameWithSpaces = "  João Silva  ";
        const string emailWithSpaces = "  joao@example.com  ";
        const string phoneWithSpaces = "  (11) 98765-4321  ";

        // Act
        var result = IndividualPerson.Create(
            _validId,
            _validTenantId,
            nameWithSpaces,
            ValidCpf,
            PersonType.Individual,
            emailWithSpaces,
            phoneWithSpaces,
            MaritalStatus.Single);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("João Silva", result.Value.Name);
        Assert.Equal("joao@example.com", result.Value.Email);
        Assert.Equal("(11) 98765-4321", result.Value.Phone);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Person_Should_Fail_When_Name_Is_Empty_Or_Whitespace(string invalidName)
    {
        // Arrange & Act
        var result = IndividualPerson.Create(
            _validId,
            _validTenantId,
            invalidName,
            ValidCpf,
            PersonType.Individual,
            ValidEmail,
            ValidPhone,
            MaritalStatus.Single);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Nome é requerido", result.Error);
    }

    [Fact]
    public void Person_Should_Throw_When_Name_Is_Null()
    {
        // Arrange & Act & Assert
        Assert.Throws<NullReferenceException>(() => IndividualPerson.Create(
            _validId,
            _validTenantId,
            null!,
            ValidCpf,
            PersonType.Individual,
            ValidEmail,
            ValidPhone,
            MaritalStatus.Single));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Person_Should_Fail_When_Email_Is_Empty_Or_Whitespace(string invalidEmail)
    {
        // Arrange & Act
        var result = IndividualPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCpf,
            PersonType.Individual,
            invalidEmail,
            ValidPhone,
            MaritalStatus.Single);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("E-mail é requerido", result.Error);
    }

    [Fact]
    public void Person_Should_Throw_When_Email_Is_Null()
    {
        // Arrange & Act & Assert
        Assert.Throws<NullReferenceException>(() => IndividualPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCpf,
            PersonType.Individual,
            null!,
            ValidPhone,
            MaritalStatus.Single));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Person_Should_Fail_When_Phone_Is_Empty_Or_Whitespace(string invalidPhone)
    {
        // Arrange & Act
        var result = IndividualPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCpf,
            PersonType.Individual,
            ValidEmail,
            invalidPhone,
            MaritalStatus.Single);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Telefone é requerido", result.Error);
    }

    [Fact]
    public void Person_Should_Throw_When_Phone_Is_Null()
    {
        // Arrange & Act & Assert
        Assert.Throws<NullReferenceException>(() => IndividualPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCpf,
            PersonType.Individual,
            ValidEmail,
            null!,
            MaritalStatus.Single));
    }

    [Fact]
    public void Person_Should_Fail_When_TenantId_Is_Empty()
    {
        // Arrange & Act
        var result = IndividualPerson.Create(
            _validId,
            Guid.Empty,
            ValidName,
            ValidCpf,
            PersonType.Individual,
            ValidEmail,
            ValidPhone,
            MaritalStatus.Single);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("TenantId é obrigatório", result.Error);
    }

    [Fact]
    public void ChangeName_Should_Update_Name_When_Valid()
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

        const string newName = "Maria Santos";

        // Act
        var result = person.ChangeName(newName);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newName, person.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null!)]
    public void ChangeName_Should_Fail_When_Name_Is_Empty_Or_Null(string? invalidName)
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
        var result = person.ChangeName(invalidName!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Nome é requerido", result.Error);
    }

    [Fact]
    public void ChangeEmail_Should_Update_Email_When_Valid()
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

        const string newEmail = "novoemail@example.com";

        // Act
        var result = person.ChangeEmail(newEmail);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newEmail, person.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null!)]
    public void ChangeEmail_Should_Fail_When_Email_Is_Empty_Or_Null(string? invalidEmail)
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
        var result = person.ChangeEmail(invalidEmail!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("E-mail é requerido", result.Error);
    }

    [Fact]
    public void ChangePhone_Should_Fail_Phone_When_Format_InValid()
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

        const string newPhone = "(11) 12345-6789";

        // Act
        var result = person.ChangePhone(newPhone);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("inválido", result.Error,StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null!)]
    public void ChangePhone_Should_Fail_When_Phone_Is_Empty_Or_Null(string? invalidPhone)
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
        var result = person.ChangePhone(invalidPhone!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Telefone é requerido", result.Error);
    }

    [Fact]
    public void ChangeName_Should_Trim_Whitespace()
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
        var result = person.ChangeName("  Novo Nome  ");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Novo Nome", person.Name);
    }

    [Fact]
    public void CreatedAt_Should_Be_Set_Automatically()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var result = IndividualPerson.Create(
            _validId,
            _validTenantId,
            ValidName,
            ValidCpf,
            PersonType.Individual,
            ValidEmail,
            ValidPhone,
            MaritalStatus.Single);

        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.CreatedAt >= beforeCreation);
        Assert.True(result.Value.CreatedAt <= afterCreation);
    }
}

