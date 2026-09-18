using System.ComponentModel.DataAnnotations;

namespace BlogEngine.Mvc.Models;

/// <summary>
/// A reader comment left on a post. No authentication is required to post one,
/// consistent with this project's "no real auth" scope.
/// </summary>
public class Comment
{
    public int Id { get; set; }

    public int PostId { get; set; }
    public Post? Post { get; set; }

    [Required, StringLength(80)]
    public string AuthorName { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Body { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
