using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

public class TagsController : BaseController
{
    private readonly ApiService _apiService;

    public TagsController(ApiService apiService)
    {
        _apiService = apiService;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        // Staff (Role 1) and Lecturer (Role 2) can manage tags
        var action = context.ActionDescriptor.RouteValues["action"];
        if (action != "Index" && action != "GetAll" && !IsStaff && !IsLecturer)
        {
            context.Result = RedirectToAction("AccessDenied", "Home");
        }
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await _apiService.GetListAsync<TagViewModel>("/api/Tags");
            return View(response?.Data ?? new List<TagViewModel>());
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading tags: {ex.Message}";
            return View(new List<TagViewModel>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var response = await _apiService.GetListAsync<TagViewModel>("/api/Tags");
            return Json(response?.Data ?? new List<TagViewModel>());
        }
        catch
        {
            return Json(new List<TagViewModel>());
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TagViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid data");
        }

        try
        {
            var response = await _apiService.PostAsync("/api/Tags", new
            {
                tagName = model.TagName,
                note = model.Note
            });

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return BadRequest(errorContent);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromBody] TagViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid data");
        }

        try
        {
            var response = await _apiService.PutAsync($"/api/Tags/{id}", new
            {
                tagName = model.TagName,
                note = model.Note
            });

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return BadRequest(errorContent);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var response = await _apiService.DeleteAsync($"/api/Tags/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { message = "Tag deleted successfully" });
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
                
                return BadRequest(new { message = "Error deleting tag" });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
