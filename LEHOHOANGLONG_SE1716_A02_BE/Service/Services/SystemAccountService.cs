using Repository.Entities;
using Repository.Interfaces;
using Service.DTOs;
using Service.Interfaces;

namespace Service.Services;

public class SystemAccountService : ISystemAccountService
{
    private readonly ISystemAccountRepository _accountRepository;

    public SystemAccountService(ISystemAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<IEnumerable<SystemAccountDto>> GetAllAccountsAsync()
    {
        var accounts = await _accountRepository.GetAllAsync();
        return accounts.Select(MapToDto);
    }

    public async Task<SystemAccountDto?> GetAccountByIdAsync(int id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        return account != null ? MapToDto(account) : null;
    }

    public async Task<SystemAccountDto> CreateAccountAsync(CreateAccountDto dto)
    {
        // Check if email already exists
        var existingAccount = await _accountRepository.GetByEmailAsync(dto.AccountEmail);
        if (existingAccount != null)
            throw new InvalidOperationException("Email already exists");

        var account = new SystemAccount
        {
            AccountName = dto.AccountName,
            AccountEmail = dto.AccountEmail,
            AccountRole = dto.AccountRole,
            AccountPassword = BCrypt.Net.BCrypt.HashPassword(dto.AccountPassword)
        };

        var created = await _accountRepository.AddAsync(account);
        return MapToDto(created);
    }

    public async Task<SystemAccountDto> UpdateAccountAsync(int id, UpdateAccountDto dto)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account == null)
            throw new KeyNotFoundException($"Account with ID {id} not found");

        // Check if email is being changed and if it already exists
        if (account.AccountEmail != dto.AccountEmail)
        {
            var existingAccount = await _accountRepository.GetByEmailAsync(dto.AccountEmail);
            if (existingAccount != null)
                throw new InvalidOperationException("Email already exists");
        }

        account.AccountName = dto.AccountName;
        account.AccountEmail = dto.AccountEmail;
        account.AccountRole = dto.AccountRole;
        
        if (!string.IsNullOrEmpty(dto.AccountPassword))
        {
            account.AccountPassword = BCrypt.Net.BCrypt.HashPassword(dto.AccountPassword);
        }

        await _accountRepository.UpdateAsync(account);
        return MapToDto(account);
    }

    public async Task<bool> DeleteAccountAsync(int id)
    {
        // Check if account has created any articles
        var hasArticles = await _accountRepository.HasCreatedArticlesAsync(id);
        if (hasArticles)
            throw new InvalidOperationException("Cannot delete account that has created news articles");

        await _accountRepository.DeleteAsync(id);
        return true;
    }

    public async Task<IEnumerable<SystemAccountDto>> SearchAccountsAsync(string searchTerm)
    {
        var accounts = await _accountRepository.FindAsync(a => 
            a.AccountName.Contains(searchTerm) || 
            a.AccountEmail.Contains(searchTerm));
        
        return accounts.Select(MapToDto);
    }

    private static SystemAccountDto MapToDto(SystemAccount account)
    {
        return new SystemAccountDto
        {
            AccountId = account.AccountId,
            AccountName = account.AccountName,
            AccountEmail = account.AccountEmail,
            AccountRole = account.AccountRole
        };
    }
}
