using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace No_Forum.Pages;

// Modellklass för Index-sidan, ärver från PageModel
public class IndexModel : PageModel
{
    // Logger för att logga information, varningar och fel
    private readonly ILogger<IndexModel> _logger;

    // Konstruktor som tar in en logger via dependency injection
    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    // Publik egenskap för att lagra mottagarens ID
    public string ReciverId { get; set; }

    // Metod som körs vid GET-förfrågningar till sidan
    public void OnGet()
    {

    }
}
