using Microsoft.AspNetCore.Mvc.RazorPages;
using No_Forum.Models;
using No_Forum.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace No_Forum.Pages
{
    // Modellklass för sidan Conversations
    public class ConversationsModel : PageModel
    {
        // Databas-koppling
        private readonly ApplicationDbContext _context;
        // Hanterar användaridentitet
        private readonly UserManager<IdentityUser> _userManager;

        // Konstruktor som tar in databas och användarhanterare
        public ConversationsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Lista med användarens konversationer (direktmeddelanden)
        public List<DM> Conversations { get; set; }

        // Körs när sidan laddas (GET)
        public async Task OnGetAsync()
        {
            // Hämtar inloggad användare
            var user = await _userManager.GetUserAsync(User);
            var userName = user?.UserName;
            var userId = user?.Id;

            // Hämtar alla meddelanden där användaren är mottagare (via användarnamn eller id)
            Conversations = await _context.DM
                .Where(dm => dm.ReciverId == userName || dm.ReciverId == userId)
                .OrderByDescending(dm => dm.CreatedAt)
                .ToListAsync();
        }
    }
}
