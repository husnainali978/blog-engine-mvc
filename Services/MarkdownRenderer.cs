using Markdig;

namespace BlogEngine.Mvc.Services;

/// <summary>
/// Markdig-backed implementation of <see cref="IMarkdownRenderer"/>.
/// The pipeline is built once and reused for every render.
/// </summary>
public class MarkdownRenderer : IMarkdownRenderer
{
    private readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseSoftlineBreakAsHardlineBreak()
        .Build();

    public string ToHtml(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return string.Empty;
        }

        return Markdown.ToHtml(markdown, _pipeline);
    }
}
