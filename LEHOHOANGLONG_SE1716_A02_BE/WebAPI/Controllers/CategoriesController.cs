using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;
using WebAPI.Extensions;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "0")] // Admin only
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // GET: api/Categories
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return this.SuccessResponse(categories, "Retrieved all categories successfully");
    }

    // GET: api/Categories/5
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
            return this.NotFoundResponse($"Category with ID {id} not found");

        return this.SuccessResponse(category, "Category retrieved successfully");
    }

    // POST: api/Categories
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return this.BadRequestResponse("Invalid category data");

        try
        {
            var category = await _categoryService.CreateCategoryAsync(dto);
            return this.CreatedResponse(category, "Category created successfully");
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequestResponse(ex.Message);
        }
    }

    // PUT: api/Categories/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return this.BadRequestResponse("Invalid category data");

        try
        {
            var category = await _categoryService.UpdateCategoryAsync(id, dto);
            return this.SuccessResponse(category, "Category updated successfully");
        }
        catch (KeyNotFoundException ex)
        {
            return this.NotFoundResponse(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequestResponse(ex.Message);
        }
    }

    // DELETE: api/Categories/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _categoryService.DeleteCategoryAsync(id);
            return this.SuccessResponse<object>(null, "Category deleted successfully");
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequestResponse(ex.Message);
        }
    }

    // GET: api/Categories/search?searchTerm=...
    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string searchTerm)
    {
        var categories = await _categoryService.SearchCategoriesAsync(searchTerm);
        return this.SuccessResponse(categories, "Search completed successfully");
    }
}
