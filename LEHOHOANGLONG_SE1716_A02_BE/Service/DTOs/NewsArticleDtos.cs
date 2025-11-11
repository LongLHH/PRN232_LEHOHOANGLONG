namespace Service.DTOs;

public class NewsArticleDto
{
    public int NewsArticleId { get; set; }
    public string NewsTitle { get; set; } = string.Empty;
    public string? Headline { get; set; }
    public string NewsContent { get; set; } = string.Empty;
    public string? NewsSource { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public bool NewsStatus { get; set; }
    public int CreatedById { get; set; }
    public string? CreatedByName { get; set; }
    public int? UpdatedById { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public List<TagDto> Tags { get; set; } = new();
}

public class CreateNewsArticleDto
{
    public string NewsTitle { get; set; } = string.Empty;
    public string? Headline { get; set; }
    public string NewsContent { get; set; } = string.Empty;
    public string? NewsSource { get; set; }
    public int CategoryId { get; set; }
    public bool NewsStatus { get; set; }
    public List<int> TagIds { get; set; } = new();
}

public class UpdateNewsArticleDto
{
    public string NewsTitle { get; set; } = string.Empty;
    public string? Headline { get; set; }
    public string NewsContent { get; set; } = string.Empty;
    public string? NewsSource { get; set; }
    public int CategoryId { get; set; }
    public bool NewsStatus { get; set; }
    public List<int> TagIds { get; set; } = new();
}

public class NewsReportDto
{
    public int NewsArticleId { get; set; }
    public string NewsTitle { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool NewsStatus { get; set; }
    public int TagCount { get; set; }
}
