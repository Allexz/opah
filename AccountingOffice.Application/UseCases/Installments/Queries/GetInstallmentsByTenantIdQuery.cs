using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.Installm.Queries.Result;

namespace AccountingOffice.Application.UseCases.Installm.Queries;

public sealed record GetInstallmentsByTenantIdQuery(Guid TenantId,
                                                    int pageNum = 1,
                                                    int pageSize = 20): IQuery<Result<IEnumerable<InstallmentResult>>>;
