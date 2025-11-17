using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.Cia.Commands;

public class UpdateCompanyCommand : ICommand<Result<bool>> 
{
    public UpdateCompanyCommand(Guid id,
                                string name,
                                string email,
                                string phone)
    {
        Id = id;
        Name = name;
        Email = email;
        Phone = phone;
    }

    public bool HasName => !string.IsNullOrWhiteSpace(Name);
    public bool HasEmail => !string.IsNullOrWhiteSpace(Email);
    public bool HasPhone => !string.IsNullOrWhiteSpace(Phone);

    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public string Phone { get; init; }
}
