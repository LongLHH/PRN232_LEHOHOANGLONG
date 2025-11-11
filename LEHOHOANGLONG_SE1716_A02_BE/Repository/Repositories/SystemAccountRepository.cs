using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Entities;
using Repository.Interfaces;

namespace Repository.Repositories;

public class SystemAccountRepository : GenericRepository<SystemAccount>, ISystemAccountRepository
{
    public SystemAccountRepository(FUNewsManagementDbContext context) : base(context)
    {
    }

    public async Task<SystemAccount?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.AccountEmail == email);
    }

    public async Task<bool> HasCreatedArticlesAsync(int accountId)
    {
        return await _context.NewsArticles.AnyAsync(n => n.CreatedById == accountId);
    }
}
