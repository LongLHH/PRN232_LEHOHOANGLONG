using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Entities;
using Repository.Interfaces;

namespace Repository.Repositories;

public class TagRepository : GenericRepository<Tag>, ITagRepository
{
    public TagRepository(FUNewsManagementDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Tag>> GetTagsByIdsAsync(IEnumerable<int> tagIds)
    {
        return await _dbSet.Where(t => tagIds.Contains(t.TagId)).ToListAsync();
    }

    public async Task<bool> HasNewsArticlesAsync(int tagId)
    {
        // Check if any NewsArticle has this tag in its Tags collection
        return await _context.NewsArticles
            .AnyAsync(n => n.Tags.Any(t => t.TagId == tagId));
    }
}
