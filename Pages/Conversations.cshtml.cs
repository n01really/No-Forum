using Microsoft.AspNetCore.Mvc.RazorPages;
using No_Forum.Models;
using No_Forum.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace No_Forum.Pages
{
    public class ConversationsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ConversationsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<DM> Conversations { get; set; }

        public async Task OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);

            // Get all messages where the user is either sender or receiver
            var allMessages = await _context.DM
                .Where(dm => dm.SenderId == userId || dm.ReciverId == userId)
                .ToListAsync();

            // Group by the other participant (not the current user)
            Conversations = allMessages
                .GroupBy(dm => dm.SenderId == userId ? dm.ReciverId : dm.SenderId)
                .Select(g => g.OrderByDescending(dm => dm.CreatedAt).First())
                .OrderByDescending(dm => dm.CreatedAt)
                .ToList();
        }
    }
}
