using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using No_Forum.Models;
using No_Forum.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace No_Forum.Pages
{
    public class SendModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SendModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        [BindProperty]
        public string ReciverId { get; set; }

        [BindProperty]
        public string Message { get; set; }

        public bool Success { get; set; }

        public async Task OnGetAsync(string ReciverId) 
        {
            var currentUserId = _userManager.GetUserId(User);
            if (!string.IsNullOrEmpty(ReciverId))
            {
                this.ReciverId = ReciverId;
            }

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Message) || string.IsNullOrWhiteSpace(ReciverId))
                return Page();

            var senderId = _userManager.GetUserId(User);
            
            var dm = new DM
            {
                SenderId = senderId,
                ReciverId = ReciverId,
                Message = Message,
                CreatedAt = DateTime.UtcNow,
                SenderName = User.Identity?.Name
            };


            _context.DM.Add(dm);
            await _context.SaveChangesAsync();
            Success = true;
            ModelState.Clear();
            return Page();
        }
    }
}
