using Repository.Entities;
using Repository.Interfaces;
using Service.DTOs;
using Service.Interfaces;

namespace Service.Services;

public class NewsArticleService : INewsArticleService
{
    private readonly INewsArticleRepository _articleRepository;
    private readonly ITagRepository _tagRepository;

    public NewsArticleService(
        INewsArticleRepository articleRepository,
        ITagRepository tagRepository)
    {
        _articleRepository = articleRepository;
        _tagRepository = tagRepository;
    }

    public async Task<IEnumerable<NewsArticleDto>> GetAllArticlesAsync()
    {
        var articles = await _articleRepository.GetAllAsync();
        var result = new List<NewsArticleDto>();

        foreach (var article in articles)
        {
            var detailedArticle = await _articleRepository.GetArticleWithDetailsAsync(article.NewsArticleId);
            if (detailedArticle != null)
                result.Add(MapToDto(detailedArticle));
        }

        return result;
    }

    public async Task<IEnumerable<NewsArticleDto>> GetActiveArticlesAsync()
    {
        var articles = await _articleRepository.GetActiveArticlesAsync();
        return articles.Select(MapToDto);
    }

    public async Task<NewsArticleDto?> GetArticleByIdAsync(int id)
    {
        var article = await _articleRepository.GetArticleWithDetailsAsync(id);
        return article != null ? MapToDto(article) : null;
    }

    public async Task<NewsArticleDto> CreateArticleAsync(CreateNewsArticleDto dto, int createdById)
    {
        var article = new NewsArticle
        {
            NewsTitle = dto.NewsTitle,
            Headline = dto.Headline,
            NewsContent = dto.NewsContent,
            NewsSource = dto.NewsSource,
            CategoryId = dto.CategoryId,
            NewsStatus = dto.NewsStatus,
            CreatedById = createdById,
            CreatedDate = DateTime.Now
        };

        // Add tags if provided
        if (dto.TagIds.Any())
        {
            var tags = await _tagRepository.GetTagsByIdsAsync(dto.TagIds);
            article.Tags = tags.ToList();
        }

        var created = await _articleRepository.AddAsync(article);
        var result = await _articleRepository.GetArticleWithDetailsAsync(created.NewsArticleId);
        return MapToDto(result!);
    }

    public async Task<NewsArticleDto> UpdateArticleAsync(int id, UpdateNewsArticleDto dto, int updatedById)
    {
        var article = await _articleRepository.GetArticleWithDetailsAsync(id);
        if (article == null)
            throw new KeyNotFoundException($"Article with ID {id} not found");

        article.NewsTitle = dto.NewsTitle;
        article.Headline = dto.Headline;
        article.NewsContent = dto.NewsContent;
        article.NewsSource = dto.NewsSource;
        article.CategoryId = dto.CategoryId;
        article.NewsStatus = dto.NewsStatus;
        article.UpdatedById = updatedById;
        article.ModifiedDate = DateTime.Now;

        // Update tags
        article.Tags.Clear();
        if (dto.TagIds.Any())
        {
            var tags = await _tagRepository.GetTagsByIdsAsync(dto.TagIds);
            foreach (var tag in tags)
            {
                article.Tags.Add(tag);
            }
        }

        await _articleRepository.UpdateAsync(article);
        var result = await _articleRepository.GetArticleWithDetailsAsync(id);
        return MapToDto(result!);
    }

    public async Task<bool> DeleteArticleAsync(int id)
    {
        var article = await _articleRepository.GetArticleWithDetailsAsync(id);
        if (article == null)
            return false;

        // Clear tags relationship before deleting
        article.Tags.Clear();
        await _articleRepository.UpdateAsync(article);
        
        // Now delete the article
        await _articleRepository.DeleteAsync(id);
        return true;
    }

    public async Task<IEnumerable<NewsArticleDto>> GetArticlesByCreatorAsync(int creatorId)
    {
        var articles = await _articleRepository.GetArticlesByCreatorAsync(creatorId);
        return articles.Select(MapToDto);
    }

    public async Task<IEnumerable<NewsReportDto>> GetArticlesReportAsync(DateTime startDate, DateTime endDate)
    {
        var articles = await _articleRepository.GetArticlesByDateRangeAsync(startDate, endDate);
        return articles.Select(article => new NewsReportDto
        {
            NewsArticleId = article.NewsArticleId,
            NewsTitle = article.NewsTitle,
            CategoryName = article.Category?.CategoryName ?? "N/A",
            CreatedByName = article.CreatedBy?.AccountName ?? "N/A",
            CreatedDate = article.CreatedDate,
            NewsStatus = article.NewsStatus,
            TagCount = article.Tags?.Count ?? 0
        });
    }

    public async Task<IEnumerable<NewsArticleDto>> SearchArticlesAsync(string searchTerm)
    {
        var articles = await _articleRepository.FindAsync(a =>
            a.NewsTitle.Contains(searchTerm) ||
            a.NewsContent.Contains(searchTerm));

        var result = new List<NewsArticleDto>();
        foreach (var article in articles)
        {
            var detailedArticle = await _articleRepository.GetArticleWithDetailsAsync(article.NewsArticleId);
            if (detailedArticle != null)
                result.Add(MapToDto(detailedArticle));
        }

        return result;
    }

    private static NewsArticleDto MapToDto(NewsArticle article)
    {
        return new NewsArticleDto
        {
            NewsArticleId = article.NewsArticleId,
            NewsTitle = article.NewsTitle,
            Headline = article.Headline,
            NewsContent = article.NewsContent,
            NewsSource = article.NewsSource,
            CategoryId = article.CategoryId,
            CategoryName = article.Category?.CategoryName,
            NewsStatus = article.NewsStatus,
            CreatedById = article.CreatedById,
            CreatedByName = article.CreatedBy?.AccountName,
            UpdatedById = article.UpdatedById,
            CreatedDate = article.CreatedDate,
            ModifiedDate = article.ModifiedDate,
            Tags = article.Tags?.Select(t => new TagDto
            {
                TagId = t.TagId,
                TagName = t.TagName,
                Note = t.Note
            }).ToList() ?? new List<TagDto>()
        };
    }
}
