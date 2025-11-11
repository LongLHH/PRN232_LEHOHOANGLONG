namespace Service.DTOs;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryDescription { get; set; }
    public int? ParentCategoryId { get; set; }
    public bool IsActive { get; set; }
}

public class CreateCategoryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryDescription { get; set; }
    public int? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateCategoryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryDescription { get; set; }
    public int? ParentCategoryId { get; set; }
    public bool IsActive { get; set; }
}
