using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace No_Forum.Pages;

// Modellklass f�r Index-sidan, �rver fr�n PageModel
public class IndexModel : PageModel
{
    // Logger f�r att logga information, varningar och fel
    private readonly ILogger<IndexModel> _logger;

    // Konstruktor som tar in en logger via dependency injection
    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    // Publik egenskap f�r att lagra mottagarens ID
    public string ReciverId { get; set; }

    // Metod som k�rs vid GET-f�rfr�gningar till sidan
    public void OnGet()
    {

    }
}
