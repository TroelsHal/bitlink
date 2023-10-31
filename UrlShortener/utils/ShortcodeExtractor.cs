using System.Text.RegularExpressions;

public static class ShortcodeExtractor
{

    private const string BaseUrl = "bitlink.azurewebsites.net/";

    public static string ExtractShortcode(string input)
    {
        // Check if input is already a simple shortcode
        if (!input.Contains("/") && !input.Contains("http"))
        {
            return input;
        }

        // Regex to match the pattern "http(s)://bitlink.azurewebsites.net/<shortcode>" or "bitlink.azurewebsites.net/<shortcode>"
        var regex = new Regex($@"(https?:\/\/)?{Regex.Escape(BaseUrl)}(?<shortcode>[A-Za-z0-9]+)");

        var match = regex.Match(input);
        if (match.Success)
        {
            return match.Groups["shortcode"].Value;
        }

        throw new ArgumentException("Invalid URL or shortcode format");
    }
}
