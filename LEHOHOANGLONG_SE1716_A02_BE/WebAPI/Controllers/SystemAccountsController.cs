using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;
using WebAPI.Extensions;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "0")] // Admin only
public class SystemAccountsController : ControllerBase
{
    private readonly ISystemAccountService _accountService;

    public SystemAccountsController(ISystemAccountService accountService)
    {
        _accountService = accountService;
    }

    // GET: api/SystemAccounts
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var accounts = await _accountService.GetAllAccountsAsync();
        return this.SuccessResponse(accounts, "Retrieved all accounts successfully");
    }

    // GET: api/SystemAccounts/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var account = await _accountService.GetAccountByIdAsync(id);
        if (account == null)
            return this.NotFoundResponse($"Account with ID {id} not found");

        return this.SuccessResponse(account, "Account retrieved successfully");
    }

    // POST: api/SystemAccounts
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountDto dto)
    {
        if (!ModelState.IsValid)
            return this.BadRequestResponse("Invalid account data");

        // Prevent creating admin accounts (role 0)
        if (dto.AccountRole == 0)
            return this.BadRequestResponse("Cannot create admin accounts. Admin role is reserved.");

        var account = await _accountService.CreateAccountAsync(dto);
        return this.CreatedResponse(account, "Account created successfully");
    }

    // PUT: api/SystemAccounts/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAccountDto dto)
    {
        if (!ModelState.IsValid)
            return this.BadRequestResponse("Invalid account data");

        // Prevent updating to admin role (role 0)
        if (dto.AccountRole == 0)
            return this.BadRequestResponse("Cannot update account to admin role. Admin role is reserved.");

        var account = await _accountService.UpdateAccountAsync(id, dto);
        return this.SuccessResponse(account, "Account updated successfully");
    }

    // DELETE: api/SystemAccounts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _accountService.DeleteAccountAsync(id);
        return this.SuccessResponse<object>(null, "Account deleted successfully");
    }

    // GET: api/SystemAccounts/search?searchTerm=...
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string searchTerm)
    {
        var accounts = await _accountService.SearchAccountsAsync(searchTerm);
        return this.SuccessResponse(accounts, "Search completed successfully");
    }
}
