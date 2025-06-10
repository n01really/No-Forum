using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using No_Forum.Data;
using No_Forum.Models;
using System.Security.Claims;

namespace No_Forum.Pages
{
    // Razor PageModel f�r att hantera profilbildsuppladdning
    public class ProfilePictureModel : PageModel
    {
        private readonly ApplicationDbContext _context; // Databas-koppling
        private readonly IWebHostEnvironment _environment; // F�r att komma �t wwwroot

        // Konstruktor som s�tter databas och milj�
        public ProfilePictureModel(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public IFormFile ProfileImage { get; set; } // Filen som laddas upp

        public string UploadResult { get; set; } // Meddelande om uppladdningsresultat

        // Hanterar POST-f�rfr�gan vid uppladdning av profilbild
        public async Task<IActionResult> OnPostAsync()
        {
            // Kontrollera att en fil har valts
            if (ProfileImage == null || ProfileImage.Length == 0)
            {
                UploadResult = "No file selected.";
                return Page();
            }

            // H�mta anv�ndarens ID fr�n claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                UploadResult = "User not found.";
                return Page();
            }

            // Skapa s�kv�g till uppladdningsmapp och filnamn
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "img", "pfp");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = $"{userId}_{Path.GetFileName(ProfileImage.FileName)}";

            // H�mta anv�ndarnamn fr�n claims
            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
            {
                UploadResult = "User not found.";
                return Page();
            }

            // Spara eller uppdatera profilbildsinformation i databasen
            var pfp = _context.PFPs.FirstOrDefault(p => p.UserId == userId);
            if (pfp == null)
            {
                pfp = new PFP { UserId = userId, UserName = userName, ProfilePicturePath = fileName };
                _context.PFPs.Add(pfp);
            }
            else
            {
                pfp.ProfilePicturePath = fileName;
                pfp.UserName = userName; // Uppdatera anv�ndarnamn om det �ndrats
                _context.PFPs.Update(pfp);
            }
            await _context.SaveChangesAsync();

            // Spara filen p� servern
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ProfileImage.CopyToAsync(stream);
            }

            UploadResult = "Profile picture uploaded successfully!";
            return Page();
        }
    }
}

