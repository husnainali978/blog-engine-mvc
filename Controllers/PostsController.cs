using BlogEngine.Mvc.Data;
using BlogEngine.Mvc.Models;
using BlogEngine.Mvc.Models.ViewModels;
using BlogEngine.Mvc.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogEngine.Mvc.Controllers;

/// <summary>
/// Public single-post view: rendered Markdown content plus the comment thread.
/// </summary>
[Route("posts")]
public class PostsController : Controller
{
    private readonly BlogDbContext _db;
    private readonly IMarkdownRenderer _markdown;

    public PostsController(BlogDbContext db, IMarkdownRenderer markdown)
    {
        _db = db;
        _markdown = markdown;
    }

    // GET: /posts/5/some-post-slug
    [HttpGet("{id:int}/{slug?}")]
    public async Task<IActionResult> Details(int id, string? slug)
    {
        var post = await _db.Posts
            .Include(p => p.Tags)
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsPublished);

        if (post is null)
        {
            return NotFound();
        }

        // Keep URLs canonical: redirect to the correct slug if it's missing or stale.
        if (!string.Equals(slug, post.Slug, StringComparison.Ordinal))
        {
            return RedirectToActionPermanent(nameof(Details), new { id = post.Id, slug = post.Slug });
        }

        var viewModel = new PostDetailsViewModel
        {
            Post = post,
            ContentHtml = _markdown.ToHtml(post.MarkdownContent),
            NewComment = new NewCommentViewModel { PostId = post.Id },
        };

        return View(viewModel);
    }

    // POST: /posts/5/some-post-slug/comments
    [HttpPost("{id:int}/{slug}/comments")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int id, string slug, NewCommentViewModel newComment)
    {
        var post = await _db.Posts
            .Include(p => p.Tags)
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsPublished);

        if (post is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            var viewModel = new PostDetailsViewModel
            {
                Post = post,
                ContentHtml = _markdown.ToHtml(post.MarkdownContent),
                NewComment = newComment,
            };
            return View(nameof(Details), viewModel);
        }

        _db.Comments.Add(new Comment
        {
            PostId = post.Id,
            AuthorName = newComment.AuthorName.Trim(),
            Body = newComment.Body.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
        });
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), controllerName: null, routeValues: new { id = post.Id, slug = post.Slug }, fragment: "comments");
    }
}
