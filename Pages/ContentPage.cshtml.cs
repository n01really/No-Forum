using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using No_Forum.Data;
using No_Forum.Models;
using No_Forum.Service;

namespace No_Forum.Pages
{


    // Modellklass f�r sidan ContentPage
    public class ContentPageModel : PageModel
    {
        // F�lt f�r databas, anv�ndarhantering och API-tj�nst
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PostsApiService _postsApiService;

        // Konstruktor som injicerar beroenden
        public ContentPageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, PostsApiService postsApiService)
        {
            _context = context;
            _userManager = userManager;
            _postsApiService = postsApiService;
        }

        // Egenskaper f�r att h�lla forumdata och inl�gg
        public Forumpages? ForumPage { get; set; }
        public List<Posts> ForumPosts { get; set; } = new();
        public Dictionary<string, IdentityUser> PostAuthors { get; set; } = new();

        // Egenskaper f�r att hantera nytt inl�gg och bild
        [BindProperty]
        public string NewPostText { get; set; } = string.Empty;

        public int ForumId { get; set; }

        [BindProperty]
        public IFormFile? NewPostImage { get; set; }

        [BindProperty]
        public int RemoveCommentId { get; set; }

        // Importerar inl�gg via API-tj�nst
        public async Task<IActionResult> OnPostImportAsync(IEnumerable<Posts> posts)
        {
            bool success = await _postsApiService.ImportPostsAsync(posts);
            // Hanterar resultatet av importen
            return Page();
        }

        // H�mtar forum och dess inl�gg vid sidladdning (GET)
        public void OnGet(int id)
        {
            ForumId = id;
            ForumPage = _context.Forumpages.FirstOrDefault(f => f.Id == id);
            ForumPosts = _context.Posts.Where(p => p.ForumpageId == id).ToList();

            // H�mtar unika anv�ndare som skapat inl�gg
            var userIds = ForumPosts
                .Where(p => !string.IsNullOrEmpty(p.CreatedBy))
                .Select(p => p.CreatedBy)
                .Distinct()
                .ToList();

            // Bygger upp en ordbok med anv�ndar-id och anv�ndarobjekt
            PostAuthors = userIds
                .Select(uid => _userManager.FindByIdAsync(uid).Result)
                .Where(u => u != null)
                .ToDictionary(u => u.Id, u => u);
        }

        // V�xlar flaggat-status p� ett inl�gg
        public async Task<IActionResult> OnPostToggleFlaggedAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
                return NotFound();

            post.Flagged = !post.Flagged;
            await _context.SaveChangesAsync();

            // Ladda om data som beh�vs f�r sidan (om det beh�vs)
            ForumId = post.ForumpageId;
            ForumPage = _context.Forumpages.FirstOrDefault(f => f.Id == ForumId);
            ForumPosts = _context.Posts.Where(p => p.ForumpageId == ForumId).ToList();

            return Page(); // Stannar kvar p� samma sida
        }
        // Hanterar nytt inl�gg (text och/eller bild)
        public async Task<IActionResult> OnPostAsync(int id)
        {
            ForumId = id;
            if (string.IsNullOrWhiteSpace(NewPostText) && NewPostImage == null)
            {
                // Laddar om sidan om inget nytt inl�gg skickas in
                ForumId = id;
                ForumPage = _context.Forumpages.FirstOrDefault(f => f.Id == id);
                ForumPosts = _context.Posts.Where(p => p.ForumpageId == id).ToList();
                return Page();
            }

            string? imagePath = null;
            // Sparar uppladdad bild om s�dan finns
            if (NewPostImage != null && NewPostImage.Length > 0)
            {
                var uploadsFolder = Path.Combine("wwwroot", "img");
                Directory.CreateDirectory(uploadsFolder); // Skapar mapp om den inte finns

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(NewPostImage.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await NewPostImage.CopyToAsync(stream);
                }

                imagePath = $"/img/{uniqueFileName}";
            }

            // Skapar nytt inl�ggsobjekt och sparar till databasen
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
