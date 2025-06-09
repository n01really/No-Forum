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
    public class ProfilePictureModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProfilePictureModel(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public IFormFile ProfileImage { get; set; }

        public string UploadResult { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ProfileImage == null || ProfileImage.Length == 0)
            {
                UploadResult = "No file selected.";
                return Page();
            }

            // Get userId from claims (adjust as needed)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                UploadResult = "User not found.";
                return Page();
            }

            // Save file to wwwroot/img/pfp
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "img", "pfp");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = $"{userId}_{Path.GetFileName(ProfileImage.FileName)}";

            // Get userName from claims
            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
            {
                UploadResult = "User not found.";
                return Page();
            }

            // Save/update PFP in database
            var pfp = _context.PFPs.FirstOrDefault(p => p.UserId == userId);
            if (pfp == null)
            {
                pfp = new PFP { UserId = userId, UserName = userName, ProfilePicturePath = fileName };
                _context.PFPs.Add(pfp);
            }
            else
            {
                pfp.ProfilePicturePath = fileName;
                pfp.UserName = userName; // Update username in case it changed
                _context.PFPs.Update(pfp);
            }
            await _context.SaveChangesAsync();

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

