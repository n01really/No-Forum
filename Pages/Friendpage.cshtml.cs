using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using No_Forum.Data;

namespace No_Forum.Pages
{
    // Modellklass f�r sidan Friendpage
    public class FriendpageModel : PageModel
    {
        // Databas-koppling
        private readonly ApplicationDbContext _context;
        // Hanterar anv�ndaridentitet
        private readonly UserManager<IdentityUser> _userManager;

        // Konstruktor som s�tter databas och anv�ndarhanterare
        public FriendpageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Lista med anv�ndarens v�nner
        public List<IdentityUser> FriendsList { get; set; } = new();

        // K�rs n�r sidan laddas (GET)
        public async Task OnGetAsync()
        {
            // H�mtar nuvarande anv�ndarens ID
            var currentUserId = _userManager.GetUserId(User);

            // H�mtar ID:n f�r alla v�nner till anv�ndaren
            var friendIds = _context.Friends
                .Where(f => f.UserId == currentUserId)
                .Select(f => f.FriendUserId)
                .ToList();

            // H�mtar anv�ndarobjekt f�r alla v�nner
            FriendsList = _context.Users
                .Where(u => friendIds.Contains(u.Id))
                .ToList();
        }
    }
}
