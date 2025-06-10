using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using No_Forum.Data;

namespace No_Forum.Pages
{
    // Modellklass för sidan Friendpage
    public class FriendpageModel : PageModel
    {
        // Databas-koppling
        private readonly ApplicationDbContext _context;
        // Hanterar användaridentitet
        private readonly UserManager<IdentityUser> _userManager;

        // Konstruktor som sätter databas och användarhanterare
        public FriendpageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Lista med användarens vänner
        public List<IdentityUser> FriendsList { get; set; } = new();

        // Körs när sidan laddas (GET)
        public async Task OnGetAsync()
        {
            // Hämtar nuvarande användarens ID
            var currentUserId = _userManager.GetUserId(User);

            // Hämtar ID:n för alla vänner till användaren
            var friendIds = _context.Friends
                .Where(f => f.UserId == currentUserId)
                .Select(f => f.FriendUserId)
                .ToList();

            // Hämtar användarobjekt för alla vänner
            FriendsList = _context.Users
                .Where(u => friendIds.Contains(u.Id))
                .ToList();
        }
    }
}
