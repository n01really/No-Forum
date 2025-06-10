using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using No_Forum.Models;
using No_Forum.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace No_Forum.Pages
{
    // PageModel f�r meddelandesidan
    public class MessageModel : PageModel
    {
        // Databas- och anv�ndarhanterare
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Konstruktor som injicerar databas och anv�ndarhanterare
        public MessageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Egenskap f�r mottagarens anv�ndar-ID (binds fr�n formul�r)
        [BindProperty]
        public string ReciverId { get; set; }

        // Egenskap f�r meddelandetext (binds fr�n formul�r)
        [BindProperty]
        public string Message { get; set; }

        // Indikerar om meddelandet skickades framg�ngsrikt
        public bool Success { get; set; }

        // Detaljer om ett specifikt meddelande
        public DM MessageDetail { get; set; }

        // Hanterar POST-f�rfr�gningar f�r att skicka meddelande
        public async Task<IActionResult> OnPostAsync()
        {
            // Kontrollera att b�de meddelande och mottagare �r angivna
            if (string.IsNullOrWhiteSpace(Message) || string.IsNullOrWhiteSpace(ReciverId))
                return Page();

            // H�mta avs�ndarens anv�ndar-ID
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

            // L�gg till meddelandet i databasen och spara
            _context.DM.Add(dm);
            await _context.SaveChangesAsync();

            // S�tt flagga f�r lyckad skickning och rensa formul�ret
            Success = true;
            ModelState.Clear();
            return Page();
        }

        // H�mtar detaljer f�r ett specifikt meddelande via dess ID (GET)
        public async Task OnGetAsync(int id)
        {
            MessageDetail = await _context.DM
                .FirstOrDefaultAsync(dm => dm.Id == id);
        }
    }
}
