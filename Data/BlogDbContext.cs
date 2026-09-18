using BlogEngine.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogEngine.Mvc.Data;

public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
    {
    }

    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasIndex(p => p.Slug).IsUnique();

            entity.HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.Tags)
                .WithMany(t => t.Posts)
                .UsingEntity<PostTag>(
                    j => j.HasOne<Tag>().WithMany().HasForeignKey(pt => pt.TagId),
                    j => j.HasOne<Post>().WithMany().HasForeignKey(pt => pt.PostId),
                    j =>
                    {
                        j.HasKey(pt => new { pt.PostId, pt.TagId });
                        j.HasData(SeedData.PostTags);
                    });
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasIndex(t => t.Slug).IsUnique();
            entity.HasData(SeedData.Tags);
        });

        modelBuilder.Entity<Comment>().HasData(SeedData.Comments);

        modelBuilder.Entity<Post>().HasData(SeedData.Posts);
    }
}
