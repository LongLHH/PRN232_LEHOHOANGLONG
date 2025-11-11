namespace Service.DTOs;

public class TagDto
{
    public int TagId { get; set; }
    public string TagName { get; set; } = string.Empty;
    public string? Note { get; set; }
}

public class CreateTagDto
{
    public string TagName { get; set; } = string.Empty;
    public string? Note { get; set; }
}

public class UpdateTagDto
{
    public string TagName { get; set; } = string.Empty;
    public string? Note { get; set; }
}
