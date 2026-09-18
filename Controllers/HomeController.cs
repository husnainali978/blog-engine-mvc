using BlogEngine.Mvc.Data;
using BlogEngine.Mvc.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogEngine.Mvc.Controllers;

/// <summary>
/// Public post listing: all published posts, optionally filtered to one tag.
/// </summary>
public class HomeController : Controller
{
    private readonly BlogDbContext _db;

    public HomeController(BlogDbContext db)
    {
        _db = db;
    }

    // GET: /  or  /?tag=ef-core
    public async Task<IActionResult> Index(string? tag)
    {
        var postsQuery = _db.Posts
            .Include(p => p.Tags)
            .Where(p => p.IsPublished)
            .AsQueryable();

        string? activeTagName = null;
        if (!string.IsNullOrWhiteSpace(tag))
        {
            postsQuery = postsQuery.Where(p => p.Tags.Any(t => t.Slug == tag));
            activeTagName = await _db.Tags
                .Where(t => t.Slug == tag)
                .Select(t => t.Name)
                .FirstOrDefaultAsync();
        }

        var posts = await postsQuery
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync();

        var tagSummaries = await _db.Tags
            .Select(t => new TagSummary
            {
                Name = t.Name,
                Slug = t.Slug,
                PostCount = t.Posts.Count(p => p.IsPublished),
            })
            .Where(t => t.PostCount > 0)
            .OrderBy(t => t.Name)
            .ToListAsync();

        var viewModel = new HomeIndexViewModel
        {
            Posts = posts,
            Tags = tagSummaries,
            ActiveTagSlug = tag,
            ActiveTagName = activeTagName,
        };

        return View(viewModel);
    }

    public IActionResult Error() => View(new Models.ErrorViewModel
    {
        RequestId = HttpContext.TraceIdentifier,
    });
}
