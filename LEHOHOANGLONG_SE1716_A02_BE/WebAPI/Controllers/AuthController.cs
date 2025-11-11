using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;
using WebAPI.Extensions;
using WebAPI.Models;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var adminEmail = _configuration["AdminAccount:Email"];
        var adminPassword = _configuration["AdminAccount:Password"];

        var result = await _authService.AdminLoginAsync(request, adminEmail!, adminPassword!);

        if (result == null)
            return this.UnauthorizedResponse("Invalid email or password");

        return this.SuccessResponse(result, "Login successful");
    }
}
