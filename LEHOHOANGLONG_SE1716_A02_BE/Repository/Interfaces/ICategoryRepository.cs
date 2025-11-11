using Repository.Entities;

namespace Repository.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<bool> HasNewsArticlesAsync(int categoryId);
}
