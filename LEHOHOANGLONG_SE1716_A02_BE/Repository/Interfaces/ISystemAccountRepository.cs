using Repository.Entities;

namespace Repository.Interfaces;

public interface ISystemAccountRepository : IGenericRepository<SystemAccount>
{
    Task<SystemAccount?> GetByEmailAsync(string email);
    Task<bool> HasCreatedArticlesAsync(int accountId);
}
