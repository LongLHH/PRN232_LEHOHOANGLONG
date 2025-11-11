using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Entities;
using Repository.Interfaces;

namespace Repository.Repositories;

public class NewsArticleRepository : GenericRepository<NewsArticle>, INewsArticleRepository
{
    public NewsArticleRepository(FUNewsManagementDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<NewsArticle>> GetActiveArticlesAsync()
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.Tags)
            .Where(n => n.NewsStatus == true)
            .ToListAsync();
    }

    public async Task<IEnumerable<NewsArticle>> GetArticlesByCreatorAsync(int creatorId)
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.Tags)
            .Where(n => n.CreatedById == creatorId)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<NewsArticle>> GetArticlesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.Tags)
            .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public async Task<NewsArticle?> GetArticleWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.Tags)
            .FirstOrDefaultAsync(n => n.NewsArticleId == id);
    }
}
