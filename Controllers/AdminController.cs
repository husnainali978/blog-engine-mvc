using BlogEngine.Mvc.Data;
using BlogEngine.Mvc.Models;
using BlogEngine.Mvc.Models.ViewModels;
using BlogEngine.Mvc.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogEngine.Mvc.Controllers;

/// <summary>
/// Post management: create, edit, delete. Deliberately not gated behind
/// authentication — this is a portfolio project, and the admin screens are
/// kept in their own controller/route so real auth could be added later by
/// wrapping this one controller rather than reworking the whole app.
/// </summary>
[Route("admin")]
public class AdminController : Controller
{
    private readonly BlogDbContext _db;

    public AdminController(BlogDbContext db)
    {
        _db = db;
    }

    // GET: /admin
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var posts = await _db.Posts
            .Include(p => p.Tags)
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync();

        return View(posts);
    }

    // GET: /admin/create
    [HttpGet("create")]
    public IActionResult Create() => View(new PostFormViewModel());

    // POST: /admin/create
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PostFormViewModel form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var post = new Post
        {
            Title = form.Title.Trim(),
            Summary = string.IsNullOrWhiteSpace(form.Summary) ? null : form.Summary.Trim(),
            MarkdownContent = form.MarkdownContent,
            Author = form.Author.Trim(),
            IsPublished = form.IsPublished,
            CreatedAtUtc = DateTime.UtcNow,
        };
        post.Slug = await GenerateUniqueSlugAsync(post.Title);
        post.Tags = await ResolveTagsAsync(form.TagsInput);

        _db.Posts.Add(post);
        await _db.SaveChangesAsync();

        TempData["StatusMessage"] = $"\"{post.Title}\" was created.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /admin/edit/5
    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _db.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
        if (post is null)
        {
            return NotFound();
        }

        var form = new PostFormViewModel
        {
            Id = post.Id,
            Title = post.Title,
            Summary = post.Summary,
            MarkdownContent = post.MarkdownContent,
            Author = post.Author,
            IsPublished = post.IsPublished,
            TagsInput = string.Join(", ", post.Tags.Select(t => t.Name)),
        };

        return View(form);
    }

    // POST: /admin/edit/5
    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PostFormViewModel form)
    {
        if (id != form.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var post = await _db.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
        if (post is null)
        {
            return NotFound();
        }

        if (!string.Equals(post.Title, form.Title.Trim(), StringComparison.Ordinal))
        {
            post.Slug = await GenerateUniqueSlugAsync(form.Title.Trim(), excludingPostId: post.Id);
        }

        post.Title = form.Title.Trim();
        post.Summary = string.IsNullOrWhiteSpace(form.Summary) ? null : form.Summary.Trim();
        post.MarkdownContent = form.MarkdownContent;
        post.Author = form.Author.Trim();
        post.IsPublished = form.IsPublished;
        post.UpdatedAtUtc = DateTime.UtcNow;

        post.Tags.Clear();
        foreach (var tag in await ResolveTagsAsync(form.TagsInput))
        {
            post.Tags.Add(tag);
        }

        await _db.SaveChangesAsync();

        TempData["StatusMessage"] = $"\"{post.Title}\" was updated.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /admin/delete/5
    [HttpGet("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _db.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
        if (post is null)
        {
            return NotFound();
        }

        return View(post);
    }

    // POST: /admin/delete/5
    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is not null)
        {
            _db.Posts.Remove(post);
            await _db.SaveChangesAsync();
            TempData["StatusMessage"] = $"\"{post.Title}\" was deleted.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<string> GenerateUniqueSlugAsync(string title, int? excludingPostId = null)
    {
        var baseSlug = SlugGenerator.Slugify(title);
        if (string.IsNullOrEmpty(baseSlug))
        {
            baseSlug = "post";
        }

        var slug = baseSlug;
        var suffix = 2;
        while (await _db.Posts.AnyAsync(p => p.Slug == slug && p.Id != excludingPostId))
        {
            slug = $"{baseSlug}-{suffix}";
            suffix++;
        }

        return slug;
    }

    private async Task<List<Tag>> ResolveTagsAsync(string? tagsInput)
    {
        var names = (tagsInput ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var resolved = new List<Tag>();
        foreach (var name in names)
        {
            var slug = SlugGenerator.Slugify(name);
            var existing = await _db.Tags.FirstOrDefaultAsync(t => t.Slug == slug);
            if (existing is not null)
            {
                resolved.Add(existing);
                continue;
            }

            var newTag = new Tag { Name = name, Slug = slug };
            _db.Tags.Add(newTag);
            resolved.Add(newTag);
        }

        return resolved;
    }
}
