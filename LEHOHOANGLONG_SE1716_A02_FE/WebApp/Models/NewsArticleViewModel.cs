using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class NewsArticleViewModel
{
    [StringLength(20)]
    [Display(Name = "Article ID")]
    public string NewsArticleId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title is required")]
    [StringLength(100)]
    [Display(Name = "Title")]
    public string NewsTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    [Display(Name = "Content")]
    public string NewsContent { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    public short CategoryId { get; set; }

    public string? CategoryName { get; set; }

    [Display(Name = "Status")]
    public bool NewsStatus { get; set; }

    public short CreatedById { get; set; }
    public string? CreatedByName { get; set; }

    [Display(Name = "Created Date")]
    public DateTime? CreatedDate { get; set; }

    [Display(Name = "Modified Date")]
    public DateTime? ModifiedDate { get; set; }

    public List<TagViewModel> Tags { get; set; } = new();

    [Display(Name = "Tags")]
    public List<int> SelectedTagIds { get; set; } = new();

    public string StatusDisplay => NewsStatus ? "Active" : "Inactive";
}

public class NewsReportViewModel
{
    public string NewsArticleId { get; set; } = string.Empty;
    public string NewsTitle { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public bool NewsStatus { get; set; }
    public int TagCount { get; set; }
    
    public string StatusDisplay => NewsStatus ? "Active" : "Inactive";
}
