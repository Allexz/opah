namespace AccountingOffice.Application.UseCases.Cia.Queries.Result;

public sealed record CompanyResult(Guid Id,
                                   string Name,
                                   string Document,
                                   string Email,
                                   string Phone,
                                   bool Active,
                                   DateTime CreatedAt);
