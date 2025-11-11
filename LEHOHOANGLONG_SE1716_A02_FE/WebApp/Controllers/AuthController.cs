using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

public class AuthController : Controller
{
    private readonly ApiService _apiService;

    public AuthController(ApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        // If already logged in, redirect to home
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await _apiService.PostAsync("/api/Auth/login", new
            {
                email = model.Email,
                password = model.Password
            });

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<LoginResponse>>(content);

                if (apiResponse?.Data != null)
                {
                    var loginResponse = apiResponse.Data;
                    
                    // Store user info in session
                    HttpContext.Session.SetString("JwtToken", loginResponse.Token);
                    HttpContext.Session.SetString("UserEmail", loginResponse.Email);
                    HttpContext.Session.SetString("UserName", loginResponse.Name);
                    HttpContext.Session.SetInt32("UserId", loginResponse.AccountId);
                    HttpContext.Session.SetInt32("UserRole", loginResponse.Role);
                    HttpContext.Session.SetInt32("AccountId", loginResponse.AccountId);

                    TempData["SuccessMessage"] = $"Welcome, {loginResponse.Name}!";

                    // Redirect based on role
                    return loginResponse.Role switch
                    {
                        0 => RedirectToAction("Index", "SystemAccounts"), // Admin
                        1 => RedirectToAction("Index", "NewsArticles"),   // Staff
                        2 => RedirectToAction("MyArticles", "NewsArticles"), // Lecturer -> My Articles
                        _ => RedirectToAction("Index", "Home")
                    };
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "Invalid email or password");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred: {ex.Message}");
        }

        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["SuccessMessage"] = "You have been logged out successfully.";
        return RedirectToAction("Index", "Home");
    }
}
