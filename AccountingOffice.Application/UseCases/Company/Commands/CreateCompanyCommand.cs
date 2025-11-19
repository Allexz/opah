using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.Cia.Commands;

public sealed record CreateCompanyCommand : ICommand<Result<Guid>>
{
    public CreateCompanyCommand(Guid id,
                                string name,
                                string document,
                                string email,
                                string phone,
                                bool active)
    {
        Id = id;
        Name = name;
        Document = document;
        Email = email;
        Phone = phone;
        Active = active;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; init; }
    public string Name { get; init; } 
    public string Document { get; init; } 
    public string Email { get; init; } 
    public string Phone { get; init; } 
    public bool Active { get; init; }
    public DateTime CreatedAt { get; init; }

}
