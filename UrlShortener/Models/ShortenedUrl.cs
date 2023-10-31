
using System;
using Microsoft.EntityFrameworkCore;

namespace shortURL.Models
{

	[Index(nameof(ShortenedCode))]
	public class ShortenedUrl
	{
		public int Id { get; set; }
		public string OriginalUrl { get; set; }
		public string? ShortenedCode { get; set; }
		public DateTime DateCreated { get; set; }
		public int ClickCount { get; set; }

		// Constructor
		public ShortenedUrl(string originalUrl)
		{
			OriginalUrl = originalUrl ?? throw new ArgumentNullException(nameof(originalUrl), "OriginalUrl cannot be null.");
			DateCreated = DateTime.UtcNow;
			ClickCount = 0;
		}
	}
}