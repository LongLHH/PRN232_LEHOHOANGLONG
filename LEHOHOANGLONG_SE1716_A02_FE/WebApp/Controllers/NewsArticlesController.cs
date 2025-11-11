using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

public class NewsArticlesController : BaseController
{
    private readonly ApiService _apiService;

    public NewsArticlesController(ApiService apiService)
    {
        _apiService = apiService;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        // Staff (Role 1) and Lecturer (Role 2) can manage news articles
        var action = context.ActionDescriptor.RouteValues["action"];
        
        // Public actions
        if (action == "Index" || action == "Details")
            return;

        // Require Staff or Lecturer
        if (!IsStaff && !IsLecturer)
        {
            context.Result = RedirectToAction("AccessDenied", "Home");
        }
    }

    public async Task<IActionResult> Index(string? searchTerm)
    {
        try
        {
            var endpoint = string.IsNullOrEmpty(searchTerm)
                ? "/api/NewsArticles"
                : $"/api/NewsArticles/search?searchTerm={searchTerm}";

            var response = await _apiService.GetListAsync<NewsArticleViewModel>(endpoint);
            
            ViewBag.SearchTerm = searchTerm;
            return View(response?.Data ?? new List<NewsArticleViewModel>());
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading articles: {ex.Message}";
            return View(new List<NewsArticleViewModel>());
        }
    }

    // My Articles - for Lecturer to see only their own articles
    public async Task<IActionResult> MyArticles()
    {
        if (!IsLecturer && !IsStaff)
        {
            return RedirectToAction("AccessDenied", "Home");
        }

        try
        {
            var response = await _apiService.GetListAsync<NewsArticleViewModel>("/api/NewsArticles/my-articles");
            return View(response?.Data ?? new List<NewsArticleViewModel>());
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading your articles: {ex.Message}";
            return View(new List<NewsArticleViewModel>());
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var article = await _apiService.GetAsync<NewsArticleViewModel>($"/api/NewsArticles/{id}");
            if (article?.Data == null)
            {
                return NotFound();
            }
            return View(article.Data);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadFormData();
        return PartialView("_CreateModal");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NewsArticleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = "Invalid data: " + string.Join(", ", errors) });
        }

        try
        {
            var response = await _apiService.PostAsync("/api/NewsArticles", new
            {
                newsTitle = model.NewsTitle,
                newsContent = model.NewsContent,
                categoryId = model.CategoryId,
                newsStatus = model.NewsStatus,
                tagIds = model.SelectedTagIds ?? new List<int>()
            });

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Article created successfully" });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, message = errorContent });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var article = await _apiService.GetAsync<NewsArticleViewModel>($"/api/NewsArticles/{id}");
            if (article?.Data == null)
            {
                return NotFound();
            }

            // Lecturer can only edit own articles
            if (IsLecturer && article.Data.CreatedById != UserId)
            {
                return Forbid();
            }

            article.Data.SelectedTagIds = article.Data.Tags.Select(t => t.TagId).ToList();
            await LoadFormData();
            return PartialView("_EditModal", article.Data);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromBody] NewsArticleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = "Invalid data: " + string.Join(", ", errors) });
        }

        try
        {
            // Check ownership for Lecturer
            if (IsLecturer)
            {
                var existing = await _apiService.GetAsync<NewsArticleViewModel>($"/api/NewsArticles/{id}");
                if (existing?.Data == null || existing.Data.CreatedById != UserId)
                {
                    return Json(new { success = false, message = "You can only edit your own articles" });
                }
            }

            var response = await _apiService.PutAsync($"/api/NewsArticles/{id}", new
            {
                newsTitle = model.NewsTitle,
                newsContent = model.NewsContent,
                categoryId = model.CategoryId,
                newsStatus = model.NewsStatus,
                tagIds = model.SelectedTagIds ?? new List<int>()
            });

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Article updated successfully" });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, message = errorContent });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var article = await _apiService.GetAsync<NewsArticleViewModel>($"/api/NewsArticles/{id}");
            return Json(article?.Data);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            Console.WriteLine($"=== DELETE ARTICLE {id} ===");
            Console.WriteLine($"User Role: {UserRole}, IsLecturer: {IsLecturer}, UserId: {UserId}");
            
            // Check ownership for Lecturer
            if (IsLecturer)
            {
                var existing = await _apiService.GetAsync<NewsArticleViewModel>($"/api/NewsArticles/{id}");
                if (existing?.Data == null || existing.Data.CreatedById != UserId)
                {
                    Console.WriteLine("Lecturer trying to delete other's article - FORBIDDEN");
                    TempData["ErrorMessage"] = "You can only delete your own articles";
                    return RedirectToAction(nameof(MyArticles));
                }
            }

            Console.WriteLine($"Calling DELETE API: /api/NewsArticles/{id}");
            var response = await _apiService.DeleteAsync($"/api/NewsArticles/{id}");
            Console.WriteLine($"API Response Status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Delete successful!");
                TempData["SuccessMessage"] = "Article deleted successfully";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Delete failed: {errorContent}");
                TempData["ErrorMessage"] = "Error deleting article: " + errorContent;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during delete: {ex.Message}");
            TempData["ErrorMessage"] = ex.Message;
        }

        return IsLecturer ? RedirectToAction(nameof(MyArticles)) : RedirectToAction(nameof(Index));
    }

    private async Task LoadFormData()
    {
        var categoriesResponse = await _apiService.GetListAsync<CategoryViewModel>("/api/Categories");
        var tagsResponse = await _apiService.GetListAsync<TagViewModel>("/api/Tags");

        ViewBag.Categories = categoriesResponse?.Data ?? new List<CategoryViewModel>();
        ViewBag.Tags = tagsResponse?.Data ?? new List<TagViewModel>();
    }
}
