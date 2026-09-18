using BlogEngine.Mvc.Data;
using BlogEngine.Mvc.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("BlogDb")
    ?? "Data Source=blog.db";
builder.Services.AddDbContext<BlogDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddSingleton<IMarkdownRenderer, MarkdownRenderer>();

var app = builder.Build();

// Create the SQLite database (and apply HasData seed values) on startup if it
// doesn't already exist. Simpler than a migrations workflow for a project
// this size, and keeps first-run setup to just "dotnet run".
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
