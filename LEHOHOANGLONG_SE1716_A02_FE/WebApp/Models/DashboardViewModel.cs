namespace WebApp.Models
{
    public class DashboardViewModel
    {
        public int TotalArticles { get; set; }
        public int ActiveArticles { get; set; }
        public int TotalCategories { get; set; }
        public int TotalTags { get; set; }
        public int TotalUsers { get; set; }
        public int TotalStaffs { get; set; }
        public int TotalLecturers { get; set; }

        public List<CategoryStat> TopCategories { get; set; } = new();
        public List<TagStat> TopTags { get; set; } = new();
        public List<AuthorStat> TopAuthors { get; set; } = new();
        public List<ArticleSummary> RecentArticles { get; set; } = new();
        public List<StatusCount> ArticlesByStatus { get; set; } = new();
        public List<CategoryCount> ArticlesByCategory { get; set; } = new();
        public List<RoleCount> UsersByRole { get; set; } = new();
    }

    public class CategoryStat
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int ArticleCount { get; set; }
    }

    public class TagStat
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = string.Empty;
        public int UsageCount { get; set; }
    }

    public class AuthorStat
    {
        public short AccountId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int ArticleCount { get; set; }
    }

    public class ArticleSummary
    {
        public string NewsArticleId { get; set; } = string.Empty;
        public string NewsTitle { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public short? CreatedById { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public bool? NewsStatus { get; set; }
    }

    public class StatusCount
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class CategoryCount
    {
        public string CategoryName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class RoleCount
    {
        public string RoleName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
