using Microsoft.AspNetCore.Mvc.RazorPages;
using No_Forum.Models;
using No_Forum.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace No_Forum.Pages
{
    // Modellklass f�r sidan Conversations
    public class ConversationsModel : PageModel
    {
        // Databas-koppling
        private readonly ApplicationDbContext _context;
        // Hanterar anv�ndaridentitet
        private readonly UserManager<IdentityUser> _userManager;

        // Konstruktor som tar in databas och anv�ndarhanterare
        public ConversationsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Lista med anv�ndarens konversationer (direktmeddelanden)
        public List<DM> Conversations { get; set; }

        // K�rs n�r sidan laddas (GET)
        public async Task OnGetAsync()
        {
            // H�mtar inloggad anv�ndare
            var user = await _userManager.GetUserAsync(User);
            var userName = user?.UserName;
            var userId = user?.Id;

            // H�mtar alla meddelanden d�r anv�ndaren �r mottagare (via anv�ndarnamn eller id)
            Conversations = await _context.DM
                .Where(dm => dm.ReciverId == userName || dm.ReciverId == userId)
                .OrderByDescending(dm => dm.CreatedAt)
                .ToListAsync();
        }
    }
}
