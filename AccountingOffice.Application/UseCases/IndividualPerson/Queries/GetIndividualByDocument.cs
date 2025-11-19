using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.Individual.Queries.Result;

namespace AccountingOffice.Application.UseCases.Individual.Queries;

public sealed record GetIndividualByDocument(string document, Guid TenantId) : IQuery<Result<IndividualPersonResult?>>;
