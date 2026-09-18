using System.ComponentModel.DataAnnotations;

namespace BlogEngine.Mvc.Models.ViewModels;

public class PostDetailsViewModel
{
    public Post Post { get; set; } = null!;
    public string ContentHtml { get; set; } = string.Empty;
    public NewCommentViewModel NewComment { get; set; } = new();
}

public class NewCommentViewModel
{
    public int PostId { get; set; }

    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(80)]
    [Display(Name = "Name")]
    public string AuthorName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Comment can't be empty.")]
    [StringLength(2000)]
    [Display(Name = "Comment")]
    public string Body { get; set; } = string.Empty;
}
