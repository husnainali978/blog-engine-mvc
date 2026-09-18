using System.ComponentModel.DataAnnotations;

namespace BlogEngine.Mvc.Models;

/// <summary>
/// A category/tag that posts can be grouped and browsed by.
/// </summary>
public class Tag
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Slug { get; set; } = string.Empty;

    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
