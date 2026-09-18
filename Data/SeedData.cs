using BlogEngine.Mvc.Models;

namespace BlogEngine.Mvc.Data;

/// <summary>
/// Fixed sample data used to seed the database via EF Core's HasData.
/// Dates and ids are hard-coded (rather than DateTime.UtcNow / auto-increment)
/// because HasData values must be deterministic across model builds.
/// </summary>
public static class SeedData
{
    public static readonly Tag[] Tags =
    [
        new() { Id = 1, Name = "ASP.NET Core", Slug = "aspnet-core" },
        new() { Id = 2, Name = "EF Core", Slug = "ef-core" },
        new() { Id = 3, Name = "Markdown", Slug = "markdown" },
        new() { Id = 4, Name = "Career", Slug = "career" },
        new() { Id = 5, Name = "Databases", Slug = "databases" },
    ];

    public static readonly Post[] Posts =
    [
        new()
        {
            Id = 1,
            Title = "Building a Blog Engine with ASP.NET Core MVC",
            Slug = "building-a-blog-engine-with-aspnet-core-mvc",
            Summary = "Why the classic Controllers + Razor views pattern is still a great choice for content-driven sites.",
            Author = "Admin",
            IsPublished = true,
            CreatedAtUtc = new DateTime(2026, 1, 5, 9, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc = null,
            MarkdownContent =
                """
                Server-rendered MVC doesn't get much attention these days, but it's still one of the
                fastest ways to ship a content-driven site that's easy to reason about.

                ## Why MVC for a blog

                - **Controllers** stay thin: they load data and pick a view.
                - **Views** stay dumb: they render what they're given, with Razor doing the HTML escaping.
                - There's no client-side routing, no build step, and no hydration story to worry about.

                ## The moving pieces

                This project has three controllers:

                1. `HomeController` — the public post listing, filterable by tag.
                2. `PostsController` — a single post's detail page, plus the comment form.
                3. `AdminController` — create/edit/delete, with no real authentication (out of scope
                   for a portfolio piece, but trivial to bolt on with ASP.NET Core Identity later).

                ```csharp
                public class PostsController : Controller
                {
                    public async Task<IActionResult> Details(int id)
                    {
                        var post = await _db.Posts
                            .Include(p => p.Tags)
                            .Include(p => p.Comments)
                            .FirstOrDefaultAsync(p => p.Id == id && p.IsPublished);

                        return post is null ? NotFound() : View(post);
                    }
                }
                ```

                Nothing exotic — just a clear separation of concerns that scales well for a blog-sized
                domain model.
                """,
        },
        new()
        {
            Id = 2,
            Title = "Why EF Core Code-First Still Rocks",
            Slug = "why-ef-core-code-first-still-rocks",
            Summary = "Modeling posts, tags, and comments as plain C# classes and letting EF Core work out the schema.",
            Author = "Admin",
            IsPublished = true,
            CreatedAtUtc = new DateTime(2026, 2, 12, 14, 30, 0, DateTimeKind.Utc),
            UpdatedAtUtc = null,
            MarkdownContent =
                """
                Code-first EF Core means the C# model *is* the source of truth. No hand-written DDL,
                no drift between "what the code expects" and "what the database has."

                ## The shape of this domain

                - `Post` has a title, a Markdown body, an author, a published flag, and timestamps.
                - `Tag` is a simple lookup, joined to `Post` through an explicit `PostTag` entity so the
                  many-to-many association rows can be seeded predictably.
                - `Comment` belongs to exactly one `Post`, with cascade delete so removing a post cleans
                  up its comments automatically.

                > A good rule of thumb: keep the join entity explicit whenever you need to seed it,
                > version it, or eventually attach extra columns (like `CreatedAtUtc` on the join row).

                ## Seeding with HasData

                `HasData` runs at model-build time, so every value has to be a constant — fixed ids,
                fixed dates, no `DateTime.UtcNow`. It's a small constraint that pays off: the seeded
                database looks identical on every machine that runs the app.
                """,
        },
        new()
        {
            Id = 3,
            Title = "Rendering Markdown Safely in Razor Views",
            Slug = "rendering-markdown-safely-in-razor-views",
            Summary = "Using Markdig to turn stored Markdown into HTML, and why that's fine to trust here.",
            Author = "Admin",
            IsPublished = true,
            CreatedAtUtc = new DateTime(2026, 3, 3, 11, 15, 0, DateTimeKind.Utc),
            UpdatedAtUtc = new DateTime(2026, 3, 4, 8, 0, 0, DateTimeKind.Utc),
            MarkdownContent =
                """
                Storing content as Markdown keeps the admin form simple — just a `<textarea>` — and
                keeps the database column human-readable.

                ## The rendering pipeline

                ```csharp
                services.AddSingleton<IMarkdownRenderer, MarkdownRenderer>();

                var pipeline = new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .Build();

                var html = Markdown.ToHtml(post.MarkdownContent, pipeline);
                ```

                The rendered HTML is written into the view with `@Html.Raw(...)`. That's normally a red
                flag — but here the only people who can write post content are the site's own admin
                screens, so there's no untrusted input reaching the renderer. If comments ever needed
                Markdown too, they'd go through a much stricter allow-list, since those *are*
                user-supplied.

                ## What Markdig buys you

                - Tables, footnotes, task lists, and auto-linking via `UseAdvancedExtensions()`.
                - Deterministic output — the same Markdown always renders the same HTML, which makes it
                  easy to unit test.
                - No dependency on a JS Markdown library or a client-side render step.
                """,
        },
        new()
        {
            Id = 4,
            Title = "Notes on Shipping Portfolio Projects",
            Slug = "notes-on-shipping-portfolio-projects",
            Summary = "Small, complete, and runnable beats big and half-finished.",
            Author = "Admin",
            IsPublished = true,
            CreatedAtUtc = new DateTime(2026, 4, 20, 16, 45, 0, DateTimeKind.Utc),
            UpdatedAtUtc = null,
            MarkdownContent =
                """
                The projects that actually land in interviews aren't the ones with the most features —
                they're the ones a reviewer can clone, run, and understand in under five minutes.

                ## A short checklist

                - `dotnet run` (after a restore) just works, with no manual database setup.
                - The README explains *why* the project exists, not just *what* it does.
                - The code reads the way you'd want a teammate's code to read.

                A blog engine is a deliberately familiar domain — that's the point. It's small enough to
                finish, but touches enough real concerns (persistence, content rendering, CRUD, basic
                validation) to say something honest about how you build software.
                """,
        },
        new()
        {
            Id = 5,
            Title = "SQLite for Small Apps: When It's the Right Call",
            Slug = "sqlite-for-small-apps",
            Summary = "A single-file database is often the most honest choice for a project this size.",
            Author = "Admin",
            IsPublished = true,
            CreatedAtUtc = new DateTime(2026, 5, 9, 10, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc = null,
            MarkdownContent =
                """
                SQLite doesn't need a server process, a connection string full of secrets, or a Docker
                container just to run the app locally. For a project like this one, that's a feature,
                not a limitation.

                ## Trade-offs, honestly

                - Great for read-heavy, low-concurrency workloads — exactly what a blog is.
                - The whole database is one file (`blog.db`), which makes resetting state as easy as
                  deleting it and re-running the app.
                - It would **not** be the right call for a write-heavy, multi-writer production system —
                  that's what SQL Server or PostgreSQL are for.

                EF Core's provider model means swapping `UseSqlite(...)` for `UseSqlServer(...)` later is
                a one-line change, not a rewrite.
                """,
        },
    ];

    public static readonly PostTag[] PostTags =
    [
        new() { PostId = 1, TagId = 1 },
        new() { PostId = 1, TagId = 4 },
        new() { PostId = 2, TagId = 2 },
        new() { PostId = 2, TagId = 5 },
        new() { PostId = 3, TagId = 3 },
        new() { PostId = 3, TagId = 1 },
        new() { PostId = 4, TagId = 4 },
        new() { PostId = 5, TagId = 5 },
        new() { PostId = 5, TagId = 2 },
    ];

    public static readonly Comment[] Comments =
    [
        new()
        {
            Id = 1,
            PostId = 1,
            AuthorName = "Sara K.",
            Body = "Clean write-up — the tag filtering approach is simple and effective.",
            CreatedAtUtc = new DateTime(2026, 1, 6, 10, 0, 0, DateTimeKind.Utc),
        },
        new()
        {
            Id = 2,
            PostId = 1,
            AuthorName = "Devon",
            Body = "Would love to see a follow-up on adding pagination to the post list.",
            CreatedAtUtc = new DateTime(2026, 1, 7, 19, 22, 0, DateTimeKind.Utc),
        },
        new()
        {
            Id = 3,
            PostId = 3,
            AuthorName = "Priya",
            Body = "Markdig's advanced extensions save a lot of manual HTML wrangling. Good call-out on the trust boundary too.",
            CreatedAtUtc = new DateTime(2026, 3, 5, 12, 40, 0, DateTimeKind.Utc),
        },
    ];
}
