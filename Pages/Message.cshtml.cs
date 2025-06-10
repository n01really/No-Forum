using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using No_Forum.Models;
using No_Forum.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace No_Forum.Pages
{
    // PageModel för meddelandesidan
    public class MessageModel : PageModel
    {
        // Databas- och användarhanterare
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Konstruktor som injicerar databas och användarhanterare
        public MessageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Egenskap för mottagarens användar-ID (binds från formulär)
        [BindProperty]
        public string ReciverId { get; set; }

        // Egenskap för meddelandetext (binds från formulär)
        [BindProperty]
        public string Message { get; set; }

        // Indikerar om meddelandet skickades framgångsrikt
        public bool Success { get; set; }

        // Detaljer om ett specifikt meddelande
        public DM MessageDetail { get; set; }

        // Hanterar POST-förfrågningar för att skicka meddelande
        public async Task<IActionResult> OnPostAsync()
        {
            // Kontrollera att både meddelande och mottagare är angivna
            if (string.IsNullOrWhiteSpace(Message) || string.IsNullOrWhiteSpace(ReciverId))
                return Page();

            // Hämta avsändarens användar-ID
            var senderId = _userManager.GetUserId(User);

            // Skapa nytt meddelandeobjekt
            var dm = new DM
            {
                SenderId = senderId,
                ReciverId = ReciverId,
                Message = Message,
                CreatedAt = DateTime.UtcNow,
                SenderName = User.Identity?.Name
            };

            // Lägg till meddelandet i databasen och spara
            _context.DM.Add(dm);
            await _context.SaveChangesAsync();

            // Sätt flagga för lyckad skickning och rensa formuläret
            Success = true;
            ModelState.Clear();
            return Page();
        }

        // Hämtar detaljer för ett specifikt meddelande via dess ID (GET)
        public async Task OnGetAsync(int id)
        {
            MessageDetail = await _context.DM
                .FirstOrDefaultAsync(dm => dm.Id == id);
        }
    }
}
