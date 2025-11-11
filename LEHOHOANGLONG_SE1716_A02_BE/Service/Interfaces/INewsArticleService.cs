using Service.DTOs;

namespace Service.Interfaces;

public interface INewsArticleService
{
    Task<IEnumerable<NewsArticleDto>> GetAllArticlesAsync();
    Task<IEnumerable<NewsArticleDto>> GetActiveArticlesAsync();
    Task<NewsArticleDto?> GetArticleByIdAsync(int id);
    Task<NewsArticleDto> CreateArticleAsync(CreateNewsArticleDto dto, int createdById);
    Task<NewsArticleDto> UpdateArticleAsync(int id, UpdateNewsArticleDto dto, int updatedById);
    Task<bool> DeleteArticleAsync(int id);
    Task<IEnumerable<NewsArticleDto>> GetArticlesByCreatorAsync(int creatorId);
    Task<IEnumerable<NewsReportDto>> GetArticlesReportAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<NewsArticleDto>> SearchArticlesAsync(string searchTerm);
}
