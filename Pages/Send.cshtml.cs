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
    // Modell f�r sidan "Send" som hanterar att skicka meddelanden mellan anv�ndare
    public class SendModel : PageModel
    {
        // Databas- och anv�ndarhanterare injiceras via konstruktorn
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Konstruktor som s�tter databas- och anv�ndarhanterare
        public SendModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Egenskap f�r mottagarens ID (binds till formul�rf�lt)
        [BindProperty]
        public string ReciverId { get; set; }

        // Egenskap f�r meddelandetexten (binds till formul�rf�lt)
        [BindProperty]
        public string Message { get; set; }

        // Flagga som indikerar om meddelandet skickades framg�ngsrikt
        public bool Success { get; set; }

        // Hanterar GET-anrop till sidan, s�tter mottagarens ID om det skickas med i URL:en
        public async Task OnGetAsync(string reciverId)
        {
            if (!string.IsNullOrEmpty(reciverId))
            {
                this.ReciverId = reciverId;
            }
        }

        // Hanterar POST-anrop n�r anv�ndaren skickar ett meddelande
        public async Task<IActionResult> OnPostAsync()
        {
            // Validerar att b�de meddelande och mottagare �r ifyllda
            if (string.IsNullOrWhiteSpace(Message) || string.IsNullOrWhiteSpace(ReciverId))
                return Page();

            // H�mtar avs�ndarens anv�ndar-ID
            var senderId = _userManager.GetUserId(User);

            // F�rs�ker hitta mottagaren via e-post eller ID
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

            // L�gger till meddelandet i databasen och sparar �ndringarna
            _context.DM.Add(dm);
            await _context.SaveChangesAsync();

            // S�tter flagga f�r lyckat skickande och rensar ModelState
            Success = true;
            ModelState.Clear();
            return Page();
        }
    }
}
