using AccountingOffice.Domain.Core.Aggregates;
using Bogus;

namespace AccountOffice.Tests.Domain.Person;

public class CompanyTests
{
    private readonly Faker _faker;
    private readonly Guid _validId;

    public CompanyTests()
    {
        _faker = new Faker("pt_BR");
        _validId = Guid.NewGuid();
    }

    [Fact]
    public void Create_Should_Succeed_With_Valid_Parameters()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var document = "12.345.678/0001-90";
        var email = _faker.Internet.Email();
        var phone = _faker.Phone.PhoneNumber();

        // Act
        var result = Company.Create(_validId, name, document, email, phone);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        var company = result.Value;
        Assert.Equal(_validId, company.Id);
        Assert.Equal(name.Trim(), company.Name);
        Assert.Equal(document.Trim(), company.Document);
        Assert.Equal(email.Trim(), company.Email);
        Assert.Equal(phone.Trim(), company.Phone);
        Assert.True(company.Active);
        Assert.True(company.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_Should_Set_Active_As_True_By_Default()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var document = "12.345.678/0001-90";
        var email = _faker.Internet.Email();
        var phone = _faker.Phone.PhoneNumber();

        // Act
        var result = Company.Create(_validId, name, document, email, phone);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Active);
    }

    [Fact]
    public void Create_Should_Set_Active_As_False_When_Specified()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var document = "12.345.678/0001-90";
        var email = _faker.Internet.Email();
        var phone = _faker.Phone.PhoneNumber();

        // Act
        var result = Company.Create(_validId, name, document, email, phone, active: false);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Value.Active);
    }

    [Fact]
    public void Create_Should_Set_CreatedAt_To_Current_Time()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var document = "12.345.678/0001-90";
        var email = _faker.Internet.Email();
        var phone = _faker.Phone.PhoneNumber();
        var beforeCreation = DateTime.UtcNow;

        // Act
        var result = Company.Create(_validId, name, document, email, phone);
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.CreatedAt >= beforeCreation);
        Assert.True(result.Value.CreatedAt <= afterCreation);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public void Create_Should_Fail_When_Name_Is_Null_Or_Whitespace(string? invalidName)
    {
        // Arrange
        var document = "12.345.678/0001-90";
        var email = _faker.Internet.Email();
        var phone = _faker.Phone.PhoneNumber();

        // Act
        var result = Company.Create(_validId, invalidName!, document, email, phone);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Nome da empresa é requerido", result.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public void Create_Should_Fail_When_Document_Is_Null_Or_Whitespace(string? invalidDocument)
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var email = _faker.Internet.Email();
        var phone = _faker.Phone.PhoneNumber();

        // Act
        var result = Company.Create(_validId, name, invalidDocument!, email, phone);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Documento da empresa é requerido", result.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public void Create_Should_Fail_When_Email_Is_Null_Or_Whitespace(string? invalidEmail)
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var document = "12.345.678/0001-90";
        var phone = _faker.Phone.PhoneNumber();

        // Act
        var result = Company.Create(_validId, name, document, invalidEmail!, phone);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("E-mail da empresa é requerido", result.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public void Create_Should_Fail_When_Phone_Is_Null_Or_Whitespace(string? invalidPhone)
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var document = "12.345.678/0001-90";
        var email = _faker.Internet.Email();

        // Act
        var result = Company.Create(_validId, name, document, email, invalidPhone!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Telefone da empresa é requerido", result.Error);
    }

    [Fact]
    public void Create_Should_Trim_Name()
    {
        // Arrange
        var nameWithSpaces = "  " + _faker.Company.CompanyName() + "  ";
        var document = "12.345.678/0001-90";
        var email = _faker.Internet.Email();
        var phone = _faker.Phone.PhoneNumber();

        // Act
        var result = Company.Create(_validId, nameWithSpaces, document, email, phone);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(nameWithSpaces.Trim(), result.Value.Name);
        Assert.NotEqual(nameWithSpaces, result.Value.Name);
    }

    [Fact]
    public void Create_Should_Trim_Document()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var documentWithSpaces = "  " + "12.345.678/0001-90" + "  ";
        var email = _faker.Internet.Email();
        var phone = _faker.Phone.PhoneNumber();

        // Act
        var result = Company.Create(_validId, name, documentWithSpaces, email, phone);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(documentWithSpaces.Trim(), result.Value.Document);
        Assert.NotEqual(documentWithSpaces, result.Value.Document);
    }

    [Fact]
    public void Create_Should_Trim_Email()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var document = "12.345.678/0001-90";
        var emailWithSpaces = "  " + _faker.Internet.Email() + "  ";
        var phone = _faker.Phone.PhoneNumber();

        // Act
        var result = Company.Create(_validId, name, document, emailWithSpaces, phone);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(emailWithSpaces.Trim(), result.Value.Email);
        Assert.NotEqual(emailWithSpaces, result.Value.Email);
    }

    [Fact]
    public void Create_Should_Trim_Phone()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var document = "12.345.678/0001-90";
        var email = _faker.Internet.Email();
        var phoneWithSpaces = "  " + _faker.Phone.PhoneNumber() + "  ";

        // Act
        var result = Company.Create(_validId, name, document, email, phoneWithSpaces);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(phoneWithSpaces.Trim(), result.Value.Phone);
        Assert.NotEqual(phoneWithSpaces, result.Value.Phone);
    }

    [Fact]
    public void UpdateName_Should_Update_Name_When_Valid()
    {
        // Arrange
        var createResult = Company.Create(
            _validId,
            _faker.Company.CompanyName(),
            "12.345.678/0001-90",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber());
        var company = createResult.Value;
        var newName = _faker.Company.CompanyName();

        // Act
        var result = company.ChangeName(newName);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newName.Trim(), company.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public void UpdateName_Should_Fail_When_Name_Is_Null_Or_Whitespace(string? invalidName)
    {
        // Arrange
        var createResult = Company.Create(
            _validId,
            _faker.Company.CompanyName(),
            "12.345.678/0001-90",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber());
        var company = createResult.Value;
        var originalName = company.Name;

        // Act
        var result = company.ChangeName(invalidName!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Nome da empresa é requerido", result.Error);
        Assert.Equal(originalName, company.Name);
    }

    [Fact]
    public void UpdateName_Should_Trim_Name()
    {
        // Arrange
        var createResult = Company.Create(
            _validId,
            _faker.Company.CompanyName(),
            "12.345.678/0001-90",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber());
        var company = createResult.Value;
        var nameWithSpaces = "  " + _faker.Company.CompanyName() + "  ";

        // Act
        var result = company.ChangeName(nameWithSpaces);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(nameWithSpaces.Trim(), company.Name);
    }

    [Fact]
    public void UpdateEmail_Should_Update_Email_When_Valid()
    {
        // Arrange
        var createResult = Company.Create(
            _validId,
            _faker.Company.CompanyName(),
            "12.345.678/0001-90",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber());
        var company = createResult.Value;
        var newEmail = _faker.Internet.Email();

        // Act
        var result = company.ChangeEmail(newEmail);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newEmail.Trim(), company.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public void UpdateEmail_Should_Fail_When_Email_Is_Null_Or_Whitespace(string? invalidEmail)
    {
        // Arrange
        var createResult = Company.Create(
            _validId,
            _faker.Company.CompanyName(),
            "12.345.678/0001-90",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber());
        var company = createResult.Value;
        var originalEmail = company.Email;

        // Act
        var result = company.ChangeEmail(invalidEmail!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("E-mail da empresa é requerido", result.Error);
        Assert.Equal(originalEmail, company.Email);
    }

    [Fact]
    public void UpdateEmail_Should_Trim_Email()
    {
        // Arrange
        var createResult = Company.Create(
            _validId,
            _faker.Company.CompanyName(),
            "12.345.678/0001-90",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber());
        var company = createResult.Value;
        var emailWithSpaces = "  " + _faker.Internet.Email() + "  ";

        // Act
        var result = company.ChangeEmail(emailWithSpaces);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(emailWithSpaces.Trim(), company.Email);
    }

    [Fact]
    public void UpdatePhone_Should_Update_Phone_When_Valid()
    {
        // Arrange
        var createResult = Company.Create(
            _validId,
            _faker.Company.CompanyName(),
            "12.345.678/0001-90",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber());
        var company = createResult.Value;
        var newPhone = _faker.Phone.PhoneNumber();

        // Act
        var result = company.ChangePhone(newPhone);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newPhone.Trim(), company.Phone);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public void UpdatePhone_Should_Fail_When_Phone_Is_Null_Or_Whitespace(string? invalidPhone)
    {
        // Arrange
        var createResult = Company.Create(
            _validId,
            _faker.Company.CompanyName(),
            "12.345.678/0001-90",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber());
        var company = createResult.Value;
        var originalPhone = company.Phone;

        // Act
        var result = company.ChangePhone(invalidPhone!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Telefone da empresa é requerido", result.Error);
        Assert.Equal(originalPhone, company.Phone);
    }

    [Fact]
    public void UpdatePhone_Should_Trim_Phone()
    {
        // Arrange
        var createResult = Company.Create(
            _validId,
            _faker.Company.CompanyName(),
            "12.345.678/0001-90",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber());
        var company = createResult.Value;
        var phoneWithSpaces = "  " + _faker.Phone.PhoneNumber() + "  ";

        // Act
        var result = company.ChangePhone(phoneWithSpaces);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(phoneWithSpaces.Trim(), company.Phone);
    }

    [Fact]
    public void Activate_Should_Set_Active_To_True()
    {
        // Arrange
        var createResult = Company.Create(
            _validId,
            _faker.Company.CompanyName(),
            "12.345.678/0001-90",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber(),
            active: false);
        var company = createResult.Value;
        Assert.False(company.Active);

        // Act
        company.Activate();

        // Assert
        Assert.True(company.Active);
    }

    [Fact]
    public void Deactivate_Should_Set_Active_To_False()
    {
        // Arrange
        var createResult = Company.Create(
            _validId,
            _faker.Company.CompanyName(),
            "12.345.678/0001-90",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber());
        var company = createResult.Value;
        Assert.True(company.Active);

        // Act
        company.Deactivate();

        // Assert
        Assert.False(company.Active);
    }

    [Fact]
    public void Company_Should_Allow_Multiple_Name_Updates()
    {
        // Arrange
        var createResult = Company.Create(
            _validId,
            _faker.Company.CompanyName(),
            "12.345.678/0001-90",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber());
        var company = createResult.Value;
        var firstName = _faker.Company.CompanyName();
        var secondName = _faker.Company.CompanyName();

        // Act
        company.ChangeName(firstName);
        company.ChangeName(secondName);

        // Assert
        Assert.Equal(secondName.Trim(), company.Name);
    }

    [Fact]
    public void Company_Should_Allow_Multiple_Email_Updates()
    {
        // Arrange
        var createResult = Company.Create(
            _validId,
            _faker.Company.CompanyName(),
            "12.345.678/0001-90",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber());
        var company = createResult.Value;
        var firstEmail = _faker.Internet.Email();
        var secondEmail = _faker.Internet.Email();

        // Act
        company.ChangeEmail(firstEmail);
        company.ChangeEmail(secondEmail);

        // Assert
        Assert.Equal(secondEmail.Trim(), company.Email);
    }

    [Fact]
    public void Company_Should_Allow_Multiple_Phone_Updates()
    {
        // Arrange
        var createResult = Company.Create(
            _validId,
            _faker.Company.CompanyName(),
            "12.345.678/0001-90",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber());
        var company = createResult.Value;
        var firstPhone = _faker.Phone.PhoneNumber();
        var secondPhone = _faker.Phone.PhoneNumber();

        // Act
        company.ChangePhone(firstPhone);
        company.ChangePhone(secondPhone);

        // Assert
        Assert.Equal(secondPhone.Trim(), company.Phone);
    }

    [Fact]
    public void Company_Should_Allow_Activate_And_Deactivate_Multiple_Times()
    {
        // Arrange
        var createResult = Company.Create(
            _validId,
            _faker.Company.CompanyName(),
            "12.345.678/0001-90",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber());
        var company = createResult.Value;

        // Act & Assert
        company.Deactivate();
        Assert.False(company.Active);

        company.Activate();
        Assert.True(company.Active);

        company.Deactivate();
        Assert.False(company.Active);

        company.Activate();
        Assert.True(company.Active);
    }

    [Fact]
    public void Company_Should_Have_Correct_Property_Types()
    {
        // Arrange & Act
        var createResult = Company.Create(
            _validId,
            _faker.Company.CompanyName(),
            "12.345.678/0001-90",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber());
        var company = createResult.Value;

        // Assert
        Assert.IsType<Guid>(company.Id);
        Assert.IsType<string>(company.Name);
        Assert.IsType<string>(company.Document);
        Assert.IsType<string>(company.Email);
        Assert.IsType<string>(company.Phone);
        Assert.IsType<DateTime>(company.CreatedAt);
        Assert.IsType<bool>(company.Active);
    }

    [Fact]
    public void Company_Should_Handle_Long_Name()
    {
        // Arrange
        var longName = _faker.Random.String2(200);
        var document = "12.345.678/0001-90";
        var email = _faker.Internet.Email();
        var phone = _faker.Phone.PhoneNumber();

        // Act
        var result = Company.Create(_validId, longName, document, email, phone);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(longName.Trim(), result.Value.Name);
    }

    [Fact]
    public void Company_Should_Handle_Long_Document()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var longDocument = _faker.Random.String2(50);
        var email = _faker.Internet.Email();
        var phone = _faker.Phone.PhoneNumber();

        // Act
        var result = Company.Create(_validId, name, longDocument, email, phone);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(longDocument.Trim(), result.Value.Document);
    }

    [Fact]
    public void Company_Should_Handle_Long_Email()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var document = "12.345.678/0001-90";
        var longEmail = _faker.Random.String2(100) + "@" + _faker.Internet.DomainName();
        var phone = _faker.Phone.PhoneNumber();

        // Act
        var result = Company.Create(_validId, name, document, longEmail, phone);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(longEmail.Trim(), result.Value.Email);
    }

    [Fact]
    public void Company_Should_Handle_Long_Phone()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var document = "12.345.678/0001-90";
        var email = _faker.Internet.Email();
        var longPhone = _faker.Random.String2(50);

        // Act
        var result = Company.Create(_validId, name, document, email, longPhone);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(longPhone.Trim(), result.Value.Phone);
    }

    [Fact]
    public void Company_Should_Preserve_Original_Values_When_Update_Fails()
    {
        // Arrange
        var createResult = Company.Create(
            _validId,
            _faker.Company.CompanyName(),
            "12.345.678/0001-90",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber());
        var company = createResult.Value;
        var originalName = company.Name;
        var originalEmail = company.Email;
        var originalPhone = company.Phone;

        // Act & Assert - Tentar atualizar com valores inválidos
        var nameResult = company.ChangeName("");
        Assert.True(nameResult.IsFailure);
        Assert.Equal(originalName, company.Name);

        var emailResult = company.ChangeEmail("");
        Assert.True(emailResult.IsFailure);
        Assert.Equal(originalEmail, company.Email);

        var phoneResult = company.ChangePhone("");
        Assert.True(phoneResult.IsFailure);
        Assert.Equal(originalPhone, company.Phone);
    }
}

