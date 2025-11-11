using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using WebAPI.Extensions;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "0")] // Admin only
public class StatisticsController : ControllerBase
{
    private readonly INewsArticleService _articleService;
    private readonly ICategoryService _categoryService;
    private readonly ITagService _tagService;
    private readonly ISystemAccountService _accountService;

    public StatisticsController(
        INewsArticleService articleService,
        ICategoryService categoryService,
        ITagService tagService,
        ISystemAccountService accountService)
    {
        _articleService = articleService;
        _categoryService = categoryService;
        _tagService = tagService;
        _accountService = accountService;
    }

    // GET: api/Statistics/dashboard
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStatistics()
    {
        try
        {
            var articles = await _articleService.GetAllArticlesAsync();
            var categories = await _categoryService.GetAllCategoriesAsync();
            var tags = await _tagService.GetAllTagsAsync();
            var accounts = await _accountService.GetAllAccountsAsync();

            var stats = new
            {
                TotalArticles = articles.Count(),
                ActiveArticles = articles.Count(a => a.NewsStatus),
                InactiveArticles = articles.Count(a => !a.NewsStatus),
                TotalCategories = categories.Count(),
                TotalTags = tags.Count(),
                TotalStaffs = accounts.Count(a => a.AccountRole == 1),
                TotalLecturers = accounts.Count(a => a.AccountRole == 2),
                TotalUsers = accounts.Count(),

                // Top 5 categories by article count
                TopCategories = articles
                    .GroupBy(a => new { a.CategoryId, a.CategoryName })
                    .Select(g => new
                    {
                        CategoryName = g.Key.CategoryName,
                        ArticleCount = g.Count()
                    })
                    .OrderByDescending(x => x.ArticleCount)
                    .Take(5)
                    .ToList(),

                // Top 5 most used tags
                TopTags = articles
                    .SelectMany(a => a.Tags)
                    .GroupBy(t => t.TagName)
                    .Select(g => new
                    {
                        TagName = g.Key,
                        UsageCount = g.Count()
                    })
                    .OrderByDescending(x => x.UsageCount)
                    .Take(5)
                    .ToList(),

                // Top 5 most productive authors
                TopAuthors = articles
                    .GroupBy(a => a.CreatedByName)
                    .Select(g => new
                    {
                        AuthorName = g.Key,
                        ArticleCount = g.Count()
                    })
                    .OrderByDescending(x => x.ArticleCount)
                    .Take(5)
                    .ToList(),

                // Recent 5 articles
                RecentArticles = articles
                    .OrderByDescending(a => a.CreatedDate)
                    .Take(5)
                    .Select(a => new
                    {
                        a.NewsArticleId,
                        a.NewsTitle,
                        a.CategoryName,
                        a.CreatedByName,
                        a.CreatedDate,
                        a.NewsStatus
                    })
                    .ToList(),

                // Articles by status for pie chart
                ArticlesByStatus = new[]
                {
                    new { Status = "Active", Count = articles.Count(a => a.NewsStatus) },
                    new { Status = "Inactive", Count = articles.Count(a => !a.NewsStatus) }
                },

                // Articles by category for bar chart
                ArticlesByCategory = articles
                    .GroupBy(a => a.CategoryName)
                    .Select(g => new
                    {
                        Category = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList(),

                // User role distribution
                UsersByRole = new[]
                {
                    new { Role = "Admin", Count = accounts.Count(a => a.AccountRole == 0) },
                    new { Role = "Staff", Count = accounts.Count(a => a.AccountRole == 1) },
                    new { Role = "Lecturer", Count = accounts.Count(a => a.AccountRole == 2) }
                }
            };

            return this.SuccessResponse(stats, "Dashboard statistics retrieved successfully");
        }
        catch (Exception ex)
        {
            return this.BadRequestResponse($"Error retrieving statistics: {ex.Message}");
        }
    }
}
