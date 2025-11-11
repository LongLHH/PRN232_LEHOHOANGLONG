using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class TagViewModel
{
    public int TagId { get; set; }

    [Required(ErrorMessage = "Tag name is required")]
    [StringLength(50)]
    [Display(Name = "Tag Name")]
    public string TagName { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Note")]
    public string? Note { get; set; }
}
