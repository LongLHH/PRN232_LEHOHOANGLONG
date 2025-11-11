using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Entities;
using Repository.Interfaces;

namespace Repository.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(FUNewsManagementDbContext context) : base(context)
    {
    }

    public async Task<bool> HasNewsArticlesAsync(int categoryId)
    {
        return await _context.NewsArticles.AnyAsync(n => n.CategoryId == categoryId);
    }
}
