using Repository.Entities;

namespace Repository.Interfaces;

public interface INewsArticleRepository : IGenericRepository<NewsArticle>
{
    Task<IEnumerable<NewsArticle>> GetActiveArticlesAsync();
    Task<IEnumerable<NewsArticle>> GetArticlesByCreatorAsync(int creatorId);
    Task<IEnumerable<NewsArticle>> GetArticlesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<NewsArticle?> GetArticleWithDetailsAsync(int id);
}
