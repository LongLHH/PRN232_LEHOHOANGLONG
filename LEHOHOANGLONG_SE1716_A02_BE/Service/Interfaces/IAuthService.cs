using Service.DTOs;

namespace Service.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse?> AdminLoginAsync(LoginRequest request, string adminEmail, string adminPassword);
    string GenerateJwtToken(string email, string name, int role, int accountId);
}
