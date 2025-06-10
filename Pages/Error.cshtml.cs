using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace No_Forum.Pages;

// Disables response caching for this page and ignores antiforgery tokens
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    // Stores the current request's unique identifier
    public string? RequestId { get; set; }

    // Indicates if the RequestId should be shown (not null or empty)
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    // Logger for error information
    private readonly ILogger<ErrorModel> _logger;

    // Constructor that injects the logger dependency
    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    // Handles GET requests; sets the RequestId for the current request
    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}

