using Microsoft.EntityFrameworkCore;
using UrlShortner.Entities;
using UrlShortner.Services;

namespace UrlShortner;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) 
        : base(options)
    {
    }

    public DbSet<ShortenedUrl> ShortenedUrls { get; set; }


    // It can work without this, but it is optimisation
    protected override void OnModelCreating (ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortenedUrl> ( builder =>
        {
            builder.Property(s => s.Code).HasMaxLength(UrlShorteningService.NumberOfCharsInShortLink);

            builder.HasIndex(s => s.Code).IsUnique();
        });
    }


}