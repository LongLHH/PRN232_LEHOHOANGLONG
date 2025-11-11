using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp.Controllers;

public class BaseController : Controller
{
    protected bool IsAuthenticated => !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));
    protected string? UserEmail => HttpContext.Session.GetString("UserEmail");
    protected string? UserName => HttpContext.Session.GetString("UserName");
    protected int? UserId => HttpContext.Session.GetInt32("UserId");
    protected int? UserRole => HttpContext.Session.GetInt32("UserRole");
    protected int? AccountId => HttpContext.Session.GetInt32("AccountId");

    protected bool IsAdmin => UserRole == 0;
    protected bool IsStaff => UserRole == 1;
    protected bool IsLecturer => UserRole == 2;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!IsAuthenticated)
        {
            context.Result = RedirectToAction("Login", "Auth");
            return;
        }

        ViewBag.IsAuthenticated = IsAuthenticated;
        ViewBag.UserEmail = UserEmail;
        ViewBag.UserName = UserName;
        ViewBag.UserId = UserId;
        ViewBag.UserRole = UserRole;
        ViewBag.IsAdmin = IsAdmin;
        ViewBag.IsStaff = IsStaff;
        ViewBag.IsLecturer = IsLecturer;

        base.OnActionExecuting(context);
    }
}
