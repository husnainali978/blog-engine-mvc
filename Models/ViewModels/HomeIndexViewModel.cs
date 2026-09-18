namespace BlogEngine.Mvc.Models.ViewModels;

public class TagSummary
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int PostCount { get; set; }
}

public class HomeIndexViewModel
{
    public IReadOnlyList<Post> Posts { get; set; } = [];
    public IReadOnlyList<TagSummary> Tags { get; set; } = [];
    public string? ActiveTagSlug { get; set; }
    public string? ActiveTagName { get; set; }
}
