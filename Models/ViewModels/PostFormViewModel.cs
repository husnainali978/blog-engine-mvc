using System.ComponentModel.DataAnnotations;

namespace BlogEngine.Mvc.Models.ViewModels;

/// <summary>
/// Backs the Admin create/edit post forms. Tags are edited as a single
/// comma-separated text field and split/matched against existing tags on save.
/// </summary>
public class PostFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A title is required.")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(400)]
    [Display(Name = "Summary")]
    public string? Summary { get; set; }

    [Required(ErrorMessage = "Post content can't be empty.")]
    [Display(Name = "Content (Markdown)")]
    public string MarkdownContent { get; set; } = string.Empty;

    [Required(ErrorMessage = "Author name is required.")]
    [StringLength(100)]
    public string Author { get; set; } = "Admin";

    [Display(Name = "Tags (comma-separated)")]
    public string? TagsInput { get; set; }

    [Display(Name = "Published")]
    public bool IsPublished { get; set; } = true;
}
