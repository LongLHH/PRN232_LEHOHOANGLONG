using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

public class SystemAccountsController : BaseController
{
    private readonly ApiService _apiService;

    public SystemAccountsController(ApiService apiService)
    {
        _apiService = apiService;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        // Only Admin can access
        if (!IsAdmin)
        {
            context.Result = RedirectToAction("AccessDenied", "Home");
        }
    }

    public async Task<IActionResult> Index(string? searchTerm)
    {
        try
        {
            var endpoint = string.IsNullOrEmpty(searchTerm)
                ? "/api/SystemAccounts"
                : $"/api/SystemAccounts/search?searchTerm={searchTerm}";

            var response = await _apiService.GetListAsync<SystemAccountViewModel>(endpoint);
            
            ViewBag.SearchTerm = searchTerm;
            return View(response?.Data ?? new List<SystemAccountViewModel>());
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading accounts: {ex.Message}";
            return View(new List<SystemAccountViewModel>());
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        return PartialView("_CreateModal");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SystemAccountViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid data" });
        }

        try
        {
            var response = await _apiService.PostAsync("/api/SystemAccounts", new
            {
                accountName = model.AccountName,
                accountEmail = model.AccountEmail,
                accountRole = model.AccountRole,
                accountPassword = model.AccountPassword
            });

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Account created successfully" });
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
            var account = await _apiService.GetAsync<SystemAccountViewModel>($"/api/SystemAccounts/{id}");
            if (account?.Data == null)
            {
                return NotFound();
            }
            return PartialView("_EditModal", account.Data);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromBody] SystemAccountViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid data" });
        }

        try
        {
            var response = await _apiService.PutAsync($"/api/SystemAccounts/{id}", new
            {
                accountName = model.AccountName,
                accountEmail = model.AccountEmail,
                accountRole = model.AccountRole,
                accountPassword = model.AccountPassword
            });

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Account updated successfully" });
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
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var response = await _apiService.DeleteAsync($"/api/SystemAccounts/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Account deleted successfully";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = "Cannot delete account with news articles";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Report()
    {
        try
        {
            var response = await _apiService.GetListAsync<SystemAccountViewModel>("/api/SystemAccounts");
            return View(response?.Data ?? new List<SystemAccountViewModel>());
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading report: {ex.Message}";
            return View(new List<SystemAccountViewModel>());
        }
    }
}
