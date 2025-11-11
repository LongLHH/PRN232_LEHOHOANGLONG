using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.Interfaces;
using Service.DTOs;
using Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Service.Services;

public class AuthService : IAuthService
{
    private readonly ISystemAccountRepository _accountRepository;
    private readonly IConfiguration _configuration;

    public AuthService(ISystemAccountRepository accountRepository, IConfiguration configuration)
    {
        _accountRepository = accountRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var account = await _accountRepository.GetByEmailAsync(request.Email);
        
        if (account == null)
            return null;

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, account.AccountPassword))
            return null;

        var token = GenerateJwtToken(account.AccountEmail, account.AccountName, account.AccountRole, account.AccountId);

        return new LoginResponse
        {
            Token = token,
            Email = account.AccountEmail,
            Name = account.AccountName,
            Role = account.AccountRole,
            AccountId = account.AccountId
        };
    }

    public async Task<LoginResponse?> AdminLoginAsync(LoginRequest request, string adminEmail, string adminPassword)
    {
        // Check admin credentials from appsettings FIRST
        if (!string.IsNullOrEmpty(adminEmail) && 
            !string.IsNullOrEmpty(adminPassword) &&
            request.Email == adminEmail && 
            request.Password == adminPassword)
        {
            var token = GenerateJwtToken(adminEmail, "Administrator", 0, 0);

            return new LoginResponse
            {
                Token = token,
                Email = adminEmail,
                Name = "Administrator",
                Role = 0,
                AccountId = 0
            };
        }

        // If not admin, check regular user accounts in database
        return await LoginAsync(request);
    }

    public string GenerateJwtToken(string email, string name, int role, int accountId)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Role, role.ToString()),
            new Claim("AccountId", accountId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(Convert.ToDouble(jwtSettings["ExpirationHours"] ?? "24")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
