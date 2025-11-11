using Service.DTOs;

namespace Service.Interfaces;

public interface ISystemAccountService
{
    Task<IEnumerable<SystemAccountDto>> GetAllAccountsAsync();
    Task<SystemAccountDto?> GetAccountByIdAsync(int id);
    Task<SystemAccountDto> CreateAccountAsync(CreateAccountDto dto);
    Task<SystemAccountDto> UpdateAccountAsync(int id, UpdateAccountDto dto);
    Task<bool> DeleteAccountAsync(int id);
    Task<IEnumerable<SystemAccountDto>> SearchAccountsAsync(string searchTerm);
}
