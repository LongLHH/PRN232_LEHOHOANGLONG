using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

public class CategoriesController : BaseController
{
    private readonly ApiService _apiService;

    public CategoriesController(ApiService apiService)
    {
        _apiService = apiService;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        // Only Admin can manage categories (Role 0)
        var action = context.ActionDescriptor.RouteValues["action"];
        if (action != "Index" && action != "GetAll" && !IsAdmin)
        {
            context.Result = RedirectToAction("AccessDenied", "Home");
        }
    }

    public async Task<IActionResult> Index(string? searchTerm)
    {
        try
        {
            var endpoint = string.IsNullOrEmpty(searchTerm)
                ? "/api/Categories"
                : $"/api/Categories/search?searchTerm={searchTerm}";

            var response = await _apiService.GetListAsync<CategoryViewModel>(endpoint);
            
            ViewBag.SearchTerm = searchTerm;
            return View(response?.Data ?? new List<CategoryViewModel>());
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading categories: {ex.Message}";
            return View(new List<CategoryViewModel>());
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        return PartialView("_CreateModal");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid data" });
        }

        try
        {
            var response = await _apiService.PostAsync("/api/Categories", new
            {
                categoryName = model.CategoryName,
                categoryDescription = model.CategoryDesciption
            });

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Category created successfully" });
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
            var category = await _apiService.GetAsync<CategoryViewModel>($"/api/Categories/{id}");
            if (category?.Data == null)
            {
                return NotFound();
            }
            return PartialView("_EditModal", category.Data);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromBody] CategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid data" });
        }

        try
        {
            var response = await _apiService.PutAsync($"/api/Categories/{id}", new
            {
                categoryName = model.CategoryName,
                categoryDescription = model.CategoryDesciption
            });

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Category updated successfully" });
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
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var response = await _apiService.GetListAsync<CategoryViewModel>("/api/Categories");
            return Json(response?.Data ?? new List<CategoryViewModel>());
        }
        catch (Exception ex)
        {
            return Json(new List<CategoryViewModel>());
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var response = await _apiService.DeleteAsync($"/api/Categories/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { message = "Category deleted successfully" });
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var errorJson = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(errorContent);
                    if (errorJson.TryGetProperty("message", out var messageProperty))
                    {
                        return BadRequest(new { message = messageProperty.GetString() });
                    }
                }
                catch { }
                
                return BadRequest(new { message = "Error deleting category" });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
