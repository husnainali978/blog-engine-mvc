namespace BlogEngine.Mvc.Services;

/// <summary>
/// Renders author-supplied Markdown into HTML for display in a view.
/// </summary>
public interface IMarkdownRenderer
{
    string ToHtml(string? markdown);
}
