using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using No_Forum.Data;
using No_Forum.Models;

namespace No_Forum.Pages
{
    public class CreatePageModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreatePageModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Forumpages ForumPage { get; set; } = new Forumpages();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!(ForumPage.Political || ForumPage.NSFW || ForumPage.Roleplay ||
                  ForumPage.Discussion || ForumPage.Meme || ForumPage.Art || ForumPage.Technology))
            {
                ModelState.AddModelError(string.Empty, "You must select at least one category.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ForumPage.CreatedAt = DateTime.UtcNow;
            ForumPage.CreatedBy = User.Identity?.Name;
            _context.Forumpages.Add(ForumPage);
            _context.SaveChanges();

            return RedirectToPage("/Index");
        }
    }
}
