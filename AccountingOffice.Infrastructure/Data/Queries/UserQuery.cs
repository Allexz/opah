using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Domain.Core.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace AccountingOffice.Infrastructure.Data.Queries;

/// <summary>
/// Serviço de consulta para usuários.
/// </summary>
public class UserQuery : IUserQuery
{
    private readonly AccountingOfficeDbContext _dbContext;

    public UserQuery(AccountingOfficeDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Busca um usuário pelo seu ID e TenantId.
    /// </summary>
    public async Task<User?> GetByIdAsync(int id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId, cancellationToken);
    }

    /// <summary>
    /// Busca todos os usuários de um tenant.
    /// </summary>
    public async Task<IEnumerable<User>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(u => u.TenantId == tenantId)
            .OrderBy(u => u.Id)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca um usuário pelo nome de usuário (UserName) e tenant.
    /// </summary>
    public async Task<User?> GetByUserNameAsync(Guid tenantId, string userName, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.TenantId == tenantId && u.UserName == userName, cancellationToken);
    }

    /// <summary>
    /// Busca usuários ativos de um tenant.
    /// </summary>
    public async Task<IEnumerable<User>> GetActiveByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(u => u.TenantId == tenantId && u.Active)
            .OrderBy(u => u.Id)
            .ToListAsync(cancellationToken);
    }
}
