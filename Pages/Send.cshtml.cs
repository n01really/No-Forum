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
    // Modell för sidan "Send" som hanterar att skicka meddelanden mellan användare
    public class SendModel : PageModel
    {
        // Databas- och användarhanterare injiceras via konstruktorn
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Konstruktor som sätter databas- och användarhanterare
        public SendModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Egenskap för mottagarens ID (binds till formulärfält)
        [BindProperty]
        public string ReciverId { get; set; }

        // Egenskap för meddelandetexten (binds till formulärfält)
        [BindProperty]
        public string Message { get; set; }

        // Flagga som indikerar om meddelandet skickades framgångsrikt
        public bool Success { get; set; }

        // Hanterar GET-anrop till sidan, sätter mottagarens ID om det skickas med i URL:en
        public async Task OnGetAsync(string reciverId)
        {
            if (!string.IsNullOrEmpty(reciverId))
            {
                this.ReciverId = reciverId;
            }
        }

        // Hanterar POST-anrop när användaren skickar ett meddelande
        public async Task<IActionResult> OnPostAsync()
        {
            // Validerar att både meddelande och mottagare är ifyllda
            if (string.IsNullOrWhiteSpace(Message) || string.IsNullOrWhiteSpace(ReciverId))
                return Page();

            // Hämtar avsändarens användar-ID
            var senderId = _userManager.GetUserId(User);

            // Försöker hitta mottagaren via e-post eller ID
            var recipient = await _userManager.FindByEmailAsync(ReciverId);
            if (recipient == null)
            {
                recipient = await _userManager.FindByIdAsync(ReciverId);
            }
            if (recipient == null)
            {
                ModelState.AddModelError(string.Empty, "Recipient not found.");
                return Page();
            }

            // Skapar ett nytt DM-objekt med meddelandeinformation
            var dm = new DM
            {
                SenderId = senderId,
                ReciverId = ReciverId,
                Message = Message,
                CreatedAt = DateTime.UtcNow,
                SenderName = User.Identity?.Name
            };

            // Lägger till meddelandet i databasen och sparar ändringarna
            _context.DM.Add(dm);
            await _context.SaveChangesAsync();

            // Sätter flagga för lyckat skickande och rensar ModelState
            Success = true;
            ModelState.Clear();
            return Page();
        }
    }
}
