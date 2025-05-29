using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using No_Forum.Models;
using No_Forum.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace No_Forum.Pages
{
    public class MessageModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MessageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public string ReciverId { get; set; }

        [BindProperty]
        public string Message { get; set; }

        public bool Success { get; set; }
        public DM MessageDetail { get; set; }



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

        public async Task OnGetAsync(int id)
        {
            MessageDetail = await _context.DM
                .FirstOrDefaultAsync(dm => dm.Id == id);
        }
    }
}
