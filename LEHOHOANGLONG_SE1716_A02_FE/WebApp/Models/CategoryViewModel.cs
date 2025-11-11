using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class CategoryViewModel
{
    public short CategoryId { get; set; }

    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100)]
    [Display(Name = "Category Name")]
    public string CategoryName { get; set; } = string.Empty;

    [StringLength(250)]
    [Display(Name = "Description")]
    public string? CategoryDesciption { get; set; }
}
