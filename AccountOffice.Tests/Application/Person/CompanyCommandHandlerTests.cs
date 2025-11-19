using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Application.UseCases.Cia.CommandHandler;
using AccountingOffice.Application.UseCases.Cia.Commands;
using AccountingOffice.Domain.Core.Aggregates;
using Bogus;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace AccountOffice.Tests.Application.Person;

public class CompanyCommandHandlerTests
{
    private readonly Faker _faker;
    private readonly Guid _companyId;
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyQuery _companyQuery;
    private readonly CompanyCommandHandler _handler;
    private readonly ILogger<CompanyCommandHandler> _logger;

    public CompanyCommandHandlerTests()
    {
        _faker = new Faker("pt_BR");
        _companyId = Guid.NewGuid();

        _companyRepository = A.Fake<ICompanyRepository>();
        _companyQuery = A.Fake<ICompanyQuery>();
        _logger = A.Fake<ILogger<CompanyCommandHandler>>();

        _handler = new CompanyCommandHandler(_companyRepository, _companyQuery,_logger);
    }

    private Company CreateValidCompany()
    {
        var result = Company.Create(
            _companyId,
            _faker.Company.CompanyName(),
            "11.222.333/0001-81",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber()
        );
        return result.Value;
    }

    [Fact]
    public async Task Handle_CreateCompanyCommand_Should_Succeed()
    {
        // Arrange
        var command = new CreateCompanyCommand(
            _companyId,
            _faker.Company.CompanyName(),
            "11.222.333/0001-81",
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber(),
            true
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.IsType<Guid>(result.Value);
        A.CallTo(() => _companyRepository.CreateAsync(A<Company>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_UpdateCompanyCommand_Should_Succeed_When_Company_Exists()
    {
        // Arrange
        var command = new UpdateCompanyCommand(
            _companyId,
            _faker.Company.CompanyName(),
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber()
        );

        var existingCompany = CreateValidCompany();

        A.CallTo(() => _companyQuery.GetByIdAsync(_companyId)).Returns(existingCompany);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        A.CallTo(() => _companyRepository.UpdateAsync(existingCompany)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_UpdateCompanyCommand_Should_Fail_When_Company_Not_Found()
    {
        // Arrange
        var command = new UpdateCompanyCommand(
            _companyId,
            _faker.Company.CompanyName(),
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber()
        );

        A.CallTo(() => _companyQuery.GetByIdAsync(_companyId)).Returns((Company?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Companhia não localizada", result.Error);
        A.CallTo(() => _companyRepository.UpdateAsync(A<Company>.Ignored)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_DeleteCompanyCommand_Should_Succeed_When_Company_Exists()
    {
        // Arrange
        var command = new DeleteCompanyCommand(_companyId);

        var existingCompany = CreateValidCompany();

        A.CallTo(() => _companyQuery.GetByIdAsync(_companyId)).Returns(existingCompany);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        A.CallTo(() => _companyRepository.DeleteAsync(_companyId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_DeleteCompanyCommand_Should_Fail_When_Company_Not_Found()
    {
        // Arrange
        var command = new DeleteCompanyCommand(_companyId);

        A.CallTo(() => _companyQuery.GetByIdAsync(_companyId)).Returns((Company?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Companhia não localizada", result.Error);
        A.CallTo(() => _companyRepository.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_ToggleCompanyActiveStatusCommand_Should_Succeed_When_Company_Exists()
    {
        // Arrange
        var command = new ToggleCompanyActiveStatusCommand(_companyId, true);

        var existingCompany = CreateValidCompany();

        A.CallTo(() => _companyQuery.GetByIdAsync(_companyId)).Returns(existingCompany);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ToggleCompanyActiveStatusCommand_Should_Fail_When_Company_Not_Found()
    {
        // Arrange
        var command = new ToggleCompanyActiveStatusCommand(_companyId, true);

        A.CallTo(() => _companyQuery.GetByIdAsync(_companyId)).Returns((Company?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Companhia não localizada", result.Error);
    }
}
