using Repository.Entities;
using Repository.Interfaces;
using Service.DTOs;
using Service.Interfaces;

namespace Service.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category != null ? MapToDto(category) : null;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        // Check for duplicate category name
        var allCategories = await _categoryRepository.GetAllAsync();
        if (allCategories.Any(c => c.CategoryName.Equals(dto.CategoryName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Category with name '{dto.CategoryName}' already exists");
        }

        var category = new Category
        {
            CategoryName = dto.CategoryName,
            CategoryDescription = dto.CategoryDescription,
            ParentCategoryId = dto.ParentCategoryId,
            IsActive = dto.IsActive
        };

        var created = await _categoryRepository.AddAsync(category);
        return MapToDto(created);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {id} not found");

        // Check for duplicate category name (excluding current category)
        var allCategories = await _categoryRepository.GetAllAsync();
        if (allCategories.Any(c => c.CategoryId != id && c.CategoryName.Equals(dto.CategoryName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Category with name '{dto.CategoryName}' already exists");
        }

        category.CategoryName = dto.CategoryName;
        category.CategoryDescription = dto.CategoryDescription;
        category.ParentCategoryId = dto.ParentCategoryId;
        category.IsActive = dto.IsActive;

        await _categoryRepository.UpdateAsync(category);
        return MapToDto(category);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        // Check if category has news articles
        var hasArticles = await _categoryRepository.HasNewsArticlesAsync(id);
        if (hasArticles)
            throw new InvalidOperationException("Cannot delete category that has news articles");

        await _categoryRepository.DeleteAsync(id);
        return true;
    }

    public async Task<IEnumerable<CategoryDto>> SearchCategoriesAsync(string searchTerm)
    {
        var categories = await _categoryRepository.FindAsync(c => 
            c.CategoryName.Contains(searchTerm) || 
            (c.CategoryDescription != null && c.CategoryDescription.Contains(searchTerm)));
        
        return categories.Select(MapToDto);
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            CategoryDescription = category.CategoryDescription,
            ParentCategoryId = category.ParentCategoryId,
            IsActive = category.IsActive
        };
    }
}
