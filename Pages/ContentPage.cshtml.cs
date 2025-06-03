using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using No_Forum.Data;
using No_Forum.Models;

namespace No_Forum.Pages
{


    public class ContentPageModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ContentPageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public Forumpages? ForumPage { get; set; }
        public List<Posts> ForumPosts { get; set; } = new();
        public Dictionary<string, IdentityUser> PostAuthors { get; set; } = new();

        [BindProperty]
        public string NewPostText { get; set; } = string.Empty;

        public int ForumId { get; set; }
        [BindProperty]
        public IFormFile? NewPostImage { get; set; }

        [BindProperty]
        public int RemoveCommentId { get; set; }

        public async Task<IActionResult> OnPostRemoveCommentAsync(int id)
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { id = ForumId });
        }

        public void OnGet(int id)
        {
            ForumId = id;
            ForumPage = _context.Forumpages.FirstOrDefault(f => f.Id == id);
            ForumPosts = _context.Posts.Where(p => p.ForumpageId == id).ToList();

            var userIds = ForumPosts
            .Where(p => !string.IsNullOrEmpty(p.CreatedBy))
            .Select(p => p.CreatedBy)
            .Distinct()
            .ToList();

            PostAuthors = userIds
                .Select(uid => _userManager.FindByIdAsync(uid).Result)
                .Where(u => u != null)
                .ToDictionary(u => u.Id, u => u);
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            ForumId = id;
            if (string.IsNullOrWhiteSpace(NewPostText) && NewPostImage == null)
            {
                // Reload page if nothing is posted
                ForumId = id;
                ForumPage = _context.Forumpages.FirstOrDefault(f => f.Id == id);
                ForumPosts = _context.Posts.Where(p => p.ForumpageId == id).ToList();
                return Page();
            }

            string? imagePath = null;
            if (NewPostImage != null && NewPostImage.Length > 0)
            {
                var uploadsFolder = Path.Combine("wwwroot", "img");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(NewPostImage.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await NewPostImage.CopyToAsync(stream);
                }

                imagePath = $"/img/{uniqueFileName}";
            }

            var post = new Posts
            {
                ForumpageId = ForumId,
                Text = NewPostText,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _userManager.GetUserId(User),
                ImagePath = imagePath
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = ForumId });
        }
       


    }
}
