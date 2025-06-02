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
            var user = await _userManager.GetUserAsync(User);
            var userName = user?.UserName;
            var userId = user?.Id;

            // Get all messages where the user is the receiver (by userName or userId)
            Conversations = await _context.DM
                .Where(dm => dm.ReciverId == userName || dm.ReciverId == userId)
                .OrderByDescending(dm => dm.CreatedAt)
                .ToListAsync();
        }
    }
}
