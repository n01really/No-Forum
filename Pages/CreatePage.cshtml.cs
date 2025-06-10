using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using No_Forum.Data;
using No_Forum.Models;

namespace No_Forum.Pages
{
    // PageModel för att hantera skapandet av nya forum-sidor
    public class CreatePageModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Konstruktor som tar in databas-kontexten via dependency injection
        public CreatePageModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Egenskap som binder formulärdata till ForumPage-modellen
        [BindProperty]
        public Forumpages ForumPage { get; set; } = new Forumpages();

        // Hanterar GET-förfrågningar (visar sidan)
        public void OnGet()
        {
        }

        // Hanterar POST-förfrågningar (när formuläret skickas in)
        public IActionResult OnPost()
        {
            // Kontrollera att minst en kategori är vald
            if (!(ForumPage.Political || ForumPage.NSFW || ForumPage.Roleplay ||
                  ForumPage.Discussion || ForumPage.Meme || ForumPage.Art || ForumPage.Technology))
            {
                ModelState.AddModelError(string.Empty, "You must select at least one category.");
            }

            // Om modellen inte är giltig, visa sidan igen med felmeddelanden
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Sätt skapandedatum och användare
            ForumPage.CreatedAt = DateTime.UtcNow;
            ForumPage.CreatedBy = User.Identity?.Name;

            // Lägg till den nya forum-sidan i databasen och spara ändringarna
            _context.Forumpages.Add(ForumPage);
            _context.SaveChanges();

            // Omdirigera till startsidan efter lyckad skapelse
            return RedirectToPage("/Index");
        }
    }
}
