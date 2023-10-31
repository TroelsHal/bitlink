using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using shortURL.Models;
using shortURL.Data;
using System.Text;


namespace shortURL.Controllers;

[Route("")]
public class HomeController : Controller
{
    private readonly UrlShortenerContext _context;
    private readonly ILogger<HomeController> _logger;


    public HomeController(ILogger<HomeController> logger, UrlShortenerContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        _logger.LogInformation("Index method called.");
        return View();
    }

    [HttpGet("About")]
    public IActionResult About()
    {
        return View();
    }

    [HttpGet("Home/Error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost("Home/ShortenUrl")]
    public IActionResult ShortenUrl(string originalUrl)
    {
        // Create a new shortened URL database-entry without the short url
        ShortenedUrl newUrl = new(originalUrl);
        _context.ShortenedUrls.Add(newUrl);
        _context.SaveChanges();


        // Keep generating random short url until a unique short code is found
        string shortCode;
        do
        {
            shortCode = GenerateRandomShortCode();
        } while (_context.ShortenedUrls.Any(u => u.ShortenedCode == shortCode));

        newUrl.ShortenedCode = shortCode;
        _context.SaveChanges();

        // Return the short URL to the user.
        return View("Index", newUrl);
    }

    [HttpGet("{input}")]
    public IActionResult RedirectToOriginal(string input)
    {

        string shortCode = ShortcodeExtractor.ExtractShortcode(input);
        if (shortCode == null)
        {
            // Handle invalid shortcode
            return View("NotFound");
        }

        var urlEntry = _context.ShortenedUrls.FirstOrDefault(u => u.ShortenedCode == shortCode);
        if (urlEntry == null)
        {
            return View("NotFound");
        }

        // Increment the click count
        urlEntry.ClickCount++;
        _context.SaveChanges();

        string originalUrl = urlEntry.OriginalUrl;
        if (!originalUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            && !originalUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            originalUrl = "http://" + originalUrl;
        }

        return Redirect(originalUrl);
    }

    [HttpGet("analytics")]
    public IActionResult Analytics()
    {
        return View();
    }

    [HttpPost("analytics")]
    public IActionResult GetAnalytics(string input)
    {
        string shortCode = ShortcodeExtractor.ExtractShortcode(input);
        if (shortCode == null)
        {
            // Handle invalid shortcode
            return View("NotFound");
        }

        var urlEntry = _context.ShortenedUrls.FirstOrDefault(u => u.ShortenedCode == shortCode);
        if (urlEntry == null)
        {
            // You might want to handle this case differently, maybe display an error message.
            return View("NotFound");
        }

        return View("AnalyticsResults", urlEntry);
    }

    // Helper method to generate a random 6-character string
    private static string GenerateRandomShortCode()
    {
        const string chars = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ23456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
