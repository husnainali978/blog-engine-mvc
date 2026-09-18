namespace BlogEngine.Mvc.Models;

/// <summary>
/// Explicit join entity for the Post &lt;-&gt; Tag many-to-many relationship.
/// Kept explicit (rather than an implicit skip-navigation join) so the
/// association rows can be seeded predictably via HasData.
/// </summary>
public class PostTag
{
    public int PostId { get; set; }
    public int TagId { get; set; }
}
