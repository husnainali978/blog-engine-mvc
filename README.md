# Blog Engine (ASP.NET Core MVC)

A server-rendered blog: write posts in Markdown, browse and read them by tag, and
manage them from a lightweight admin screen — all built on classic ASP.NET Core
MVC (Controllers + Razor views, no SPA, no API layer). It exists to demonstrate
the fundamentals most full-stack .NET work still runs on: server-rendered views,
the MVC request/response cycle, model binding and validation, and turning
stored content (Markdown) into safe, styled HTML at render time.

## Architecture

Standard MVC layering, kept intentionally small:

- **Models** (`Models/`) — `Post`, `Tag`, `Comment`, and an explicit `PostTag`
  join entity for the many-to-many post/tag relationship. `Models/ViewModels/`
  holds the shapes the views actually bind to (e.g. `PostFormViewModel`, whose
  `TagsInput` is a comma-separated string rather than a raw collection).
- **Data** (`Data/`) — `BlogDbContext` (EF Core + SQLite) and `SeedData`, a set
  of fixed sample posts/tags/comments loaded via `HasData` so the app has
  content the moment it starts.
- **Services** (`Services/`) — `IMarkdownRenderer` wraps Markdig behind an
  interface so controllers don't depend on the library directly; `SlugGenerator`
  turns titles into URL-friendly slugs.
- **Controllers** (`Controllers/`)
  - `HomeController` — public post listing, filterable by tag.
  - `PostsController` — a single post's detail page (rendered Markdown +
    comment thread) and the comment submission endpoint.
  - `AdminController` — create/edit/delete for posts. There's no login: it's a
    separate controller and route (`/admin/...`) rather than a hidden flag on
    the public one, so real authentication could be layered on top of just
    this controller later without touching anything else.
- **Views** (`Views/`) — Razor views per controller, a shared `_Layout.cshtml`,
  and a couple of partials (`_PostFormFields`, `_ValidationScriptsPartial`) so
  the create/edit forms don't duplicate markup.

Posts are stored as raw Markdown. `PostsController` renders that Markdown to
HTML once per request via Markdig and hands the view a ready-to-display
`ContentHtml` string — the view just does `@Html.Raw(...)`, since the only
thing that ever writes post content is the site's own admin screens.

## Features

- Public post list with tag filtering (`/?tag=ef-core`) and a tag sidebar
  showing post counts.
- Post detail pages with SEO-friendly URLs (`/posts/{id}/{slug}`), Markdown
  rendered to HTML (headings, code fences, tables, lists, blockquotes, etc.),
  and a comment thread with a submission form (no login required).
- Admin area (`/admin`) to create, edit, and delete posts:
  - Tags are entered as a comma-separated field and resolved/created
    automatically — no separate tag-management screen needed.
  - Slugs are generated from the title and kept unique.
  - Posts can be saved as drafts (`IsPublished = false`) and won't appear on
    the public site until published.
- Server-side model validation with the standard ASP.NET Core tag helpers
  (`asp-validation-for`, antiforgery tokens on every form post).
- EF Core + SQLite persistence, seeded with five sample posts, five tags, and
  a few comments via `HasData` — the database is created automatically on
  first run.

## How to run it

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
cd blog-engine-mvc
dotnet restore
dotnet run
```

Then open the URL printed in the console (typically `http://localhost:5171`).
The SQLite database (`blog.db`) is created and seeded automatically the first
time the app starts — no manual migration step required. Delete `blog.db` and
restart the app at any point to reset back to the seed data.

> If your machine has a broken/unreachable NuGet source configured globally,
> restore explicitly against nuget.org instead:
> `dotnet restore -s https://api.nuget.org/v3/index.json`.

## Tech stack

ASP.NET Core 10 MVC (Controllers + Razor views), EF Core with SQLite, Markdig
for Markdown rendering, Bootstrap 5 (via CDN) for styling.
