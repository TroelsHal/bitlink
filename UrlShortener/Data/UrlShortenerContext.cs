namespace shortURL.Data
{
    using Microsoft.EntityFrameworkCore;
    using shortURL.Models;

    public class UrlShortenerContext : DbContext
    {
        public UrlShortenerContext(DbContextOptions<UrlShortenerContext> options) : base(options) { }

        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }
    }
}