using AccountingOffice.Domain.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingOffice.Application.Interfaces.Repositories;

public interface IInstalmentRepository
{
    Task<bool> CreateAsync(Installment installment);
    Task<bool> DeleteAsync(Guid accountId, Guid tenantId, int installmentNumber);
}
