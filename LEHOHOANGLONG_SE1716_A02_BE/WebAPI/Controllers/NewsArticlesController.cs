using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;
using System.Security.Claims;
using WebAPI.Extensions;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsArticlesController : ControllerBase
{
    private readonly INewsArticleService _articleService;

    public NewsArticlesController(INewsArticleService articleService)
    {
        _articleService = articleService;
    }

    // GET: api/NewsArticles
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // If not authenticated, only return active articles
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            var activeArticles = await _articleService.GetActiveArticlesAsync();
            return this.SuccessResponse(activeArticles, "Retrieved active articles successfully");
        }

        var articles = await _articleService.GetAllArticlesAsync();
        return this.SuccessResponse(articles, "Retrieved all articles successfully");
    }

    // GET: api/NewsArticles/5
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var article = await _articleService.GetArticleByIdAsync(id);
        if (article == null)
            return this.NotFoundResponse($"Article with ID {id} not found");

        // If not authenticated, only return active articles
        if ((!User.Identity?.IsAuthenticated ?? true) && !article.NewsStatus)
            return this.ForbiddenResponse("This article is not active");

        return this.SuccessResponse(article, "Article retrieved successfully");
    }

    // POST: api/NewsArticles
    [Authorize(Roles = "1,2")] // Staff and Lecturer
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNewsArticleDto dto)
    {
        if (!ModelState.IsValid)
            return this.BadRequestResponse("Invalid article data");

        var accountId = int.Parse(User.FindFirst("AccountId")?.Value ?? "0");
        var article = await _articleService.CreateArticleAsync(dto, accountId);
        return this.CreatedResponse(article, "Article created successfully");
    }

    // PUT: api/NewsArticles/5
    [Authorize(Roles = "1,2")] // Staff and Lecturer
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateNewsArticleDto dto)
    {
        if (!ModelState.IsValid)
            return this.BadRequestResponse("Invalid article data");

        var accountId = int.Parse(User.FindFirst("AccountId")?.Value ?? "0");
        var userRole = int.Parse(User.FindFirst(ClaimTypes.Role)?.Value ?? "0");
        
        // If Lecturer (role 2), verify ownership
        if (userRole == 2)
        {
            var existingArticle = await _articleService.GetArticleByIdAsync(id);
            if (existingArticle == null)
                return this.NotFoundResponse($"Article with ID {id} not found");
            
            if (existingArticle.CreatedById != accountId)
                return this.ForbiddenResponse("You can only edit your own articles");
        }

        var article = await _articleService.UpdateArticleAsync(id, dto, accountId);
        return this.SuccessResponse(article, "Article updated successfully");
    }

    // DELETE: api/NewsArticles/5
    [Authorize(Roles = "1,2")] // Staff and Lecturer
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var accountId = int.Parse(User.FindFirst("AccountId")?.Value ?? "0");
        var userRole = int.Parse(User.FindFirst(ClaimTypes.Role)?.Value ?? "0");
        
        // If Lecturer (role 2), verify ownership
        if (userRole == 2)
        {
            var existingArticle = await _articleService.GetArticleByIdAsync(id);
            if (existingArticle == null)
                return this.NotFoundResponse($"Article with ID {id} not found");
            
            if (existingArticle.CreatedById != accountId)
                return this.ForbiddenResponse("You can only delete your own articles");
        }

        await _articleService.DeleteArticleAsync(id);
        return this.SuccessResponse<object>(null, "Article deleted successfully");
    }

    // GET: api/NewsArticles/my-articles
    [Authorize(Roles = "1,2")] // Staff and Lecturer
    [HttpGet("my-articles")]
    public async Task<IActionResult> GetMyArticles()
    {
        var accountId = int.Parse(User.FindFirst("AccountId")?.Value ?? "0");
        var articles = await _articleService.GetArticlesByCreatorAsync(accountId);
        return this.SuccessResponse(articles, "Retrieved my articles successfully");
    }

    // GET: api/NewsArticles/report?startDate=...&endDate=...
    [Authorize(Roles = "0")] // Admin only
    [HttpGet("report")]
    public async Task<IActionResult> GetReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var report = await _articleService.GetArticlesReportAsync(startDate, endDate);
        return this.SuccessResponse(report, "Report generated successfully");
    }

    // GET: api/NewsArticles/search?searchTerm=...
    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string searchTerm)
    {
        var articles = await _articleService.SearchArticlesAsync(searchTerm);
        
        // If not authenticated, only return active articles
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            articles = articles.Where(a => a.NewsStatus);
        }

        return this.SuccessResponse(articles, "Search completed successfully");
    }
}
