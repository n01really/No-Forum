using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using No_Forum.Data;
using No_Forum.Models;

namespace No_Forum.Pages
{
    // PageModel f�r att hantera skapandet av nya forum-sidor
    public class CreatePageModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Konstruktor som tar in databas-kontexten via dependency injection
        public CreatePageModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Egenskap som binder formul�rdata till ForumPage-modellen
        [BindProperty]
        public Forumpages ForumPage { get; set; } = new Forumpages();

        // Hanterar GET-f�rfr�gningar (visar sidan)
        public void OnGet()
        {
        }

        // Hanterar POST-f�rfr�gningar (n�r formul�ret skickas in)
        public IActionResult OnPost()
        {
            // Kontrollera att minst en kategori �r vald
            if (!(ForumPage.Political || ForumPage.NSFW || ForumPage.Roleplay ||
                  ForumPage.Discussion || ForumPage.Meme || ForumPage.Art || ForumPage.Technology))
            {
                ModelState.AddModelError(string.Empty, "You must select at least one category.");
            }

            // Om modellen inte �r giltig, visa sidan igen med felmeddelanden
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // S�tt skapandedatum och anv�ndare
            ForumPage.CreatedAt = DateTime.UtcNow;
            ForumPage.CreatedBy = User.Identity?.Name;

            // L�gg till den nya forum-sidan i databasen och spara �ndringarna
            _context.Forumpages.Add(ForumPage);
            _context.SaveChanges();

            // Omdirigera till startsidan efter lyckad skapelse
            return RedirectToPage("/Index");
        }
    }
}
