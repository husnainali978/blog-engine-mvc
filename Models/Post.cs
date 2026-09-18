using System.ComponentModel.DataAnnotations;

namespace BlogEngine.Mvc.Models;

/// <summary>
/// A blog post. Content is authored and stored as Markdown and rendered to
/// HTML at read time by <see cref="Services.IMarkdownRenderer"/>.
/// </summary>
public class Post
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>URL-friendly identifier used in the public post URL.</summary>
    [Required, StringLength(200)]
    public string Slug { get; set; } = string.Empty;

    [StringLength(400)]
    public string? Summary { get; set; }

    [Required]
    public string MarkdownContent { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Author { get; set; } = "Admin";

    public bool IsPublished { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
