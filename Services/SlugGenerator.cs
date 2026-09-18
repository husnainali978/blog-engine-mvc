using System.Text;
using System.Text.RegularExpressions;

namespace BlogEngine.Mvc.Services;

/// <summary>
/// Turns free-text titles/tag names into URL-friendly slugs.
/// </summary>
public static partial class SlugGenerator
{
    public static string Slugify(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Trim().ToLowerInvariant();
        normalized = InvalidCharsRegex().Replace(normalized, "-");
        normalized = MultipleDashesRegex().Replace(normalized, "-");
        return normalized.Trim('-');
    }

    [GeneratedRegex(@"[^a-z0-9]+")]
    private static partial Regex InvalidCharsRegex();

    [GeneratedRegex(@"-{2,}")]
    private static partial Regex MultipleDashesRegex();
}
