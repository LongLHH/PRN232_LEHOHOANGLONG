using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApiService _apiService;

    public HomeController(ILogger<HomeController> logger, ApiService apiService)
    {
        _logger = logger;
        _apiService = apiService;
    }

    public async Task<IActionResult> Index(string? searchTerm)
    {
        try
        {
            // Set ViewBag values first
            ViewBag.SearchTerm = searchTerm;
            ViewBag.IsAuthenticated = !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserRole = HttpContext.Session.GetInt32("UserRole");
            
            // If admin, redirect to dashboard
            if (ViewBag.UserRole == 0)
            {
                return RedirectToAction("Dashboard");
            }
            
            // Public can only see active articles
            var endpoint = string.IsNullOrEmpty(searchTerm)
                ? "/api/NewsArticles"
                : $"/api/NewsArticles/search?searchTerm={searchTerm}";

            var response = await _apiService.GetListAsync<NewsArticleViewModel>(endpoint);
            
            return View(response?.Data ?? new List<NewsArticleViewModel>());
        }
        catch (Exception ex)
        {
            ViewBag.SearchTerm = searchTerm;
            ViewBag.IsAuthenticated = !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserRole = HttpContext.Session.GetInt32("UserRole");
            
            TempData["ErrorMessage"] = $"Error loading articles: {ex.Message}";
            return View(new List<NewsArticleViewModel>());
        }
    }

    public async Task<IActionResult> Dashboard()
    {
        ViewBag.IsAuthenticated = !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));
        ViewBag.UserName = HttpContext.Session.GetString("UserName");
        ViewBag.UserRole = HttpContext.Session.GetInt32("UserRole");
        
        // Only admin can access dashboard
        if (ViewBag.UserRole != 0)
        {
            return RedirectToAction("AccessDenied");
        }

        try
        {
            var response = await _apiService.GetAsync<DashboardViewModel>("/api/Statistics/dashboard");
            if (response?.Data == null)
            {
                TempData["ErrorMessage"] = "Unable to load dashboard data";
                return View(new DashboardViewModel());
            }

            return View(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            TempData["ErrorMessage"] = $"Error loading dashboard: {ex.Message}";
            return View(new DashboardViewModel());
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            ViewBag.IsAuthenticated = !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserRole = HttpContext.Session.GetInt32("UserRole");
            
            var article = await _apiService.GetAsync<NewsArticleViewModel>($"/api/NewsArticles/{id}");
            if (article?.Data == null)
            {
                return NotFound();
            }

            return View(article.Data);
        }
        catch
        {
            ViewBag.IsAuthenticated = !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));
            return NotFound();
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
