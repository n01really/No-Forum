using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using No_Forum.Models;
using No_Forum.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace No_Forum.Pages
{
    // Modellklass för sidan ContentPageViewer
    public class ContentPageViewerModel : PageModel
    {
        // Databas- och användarhanterare
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Konstruktor som injicerar databas och användarhanterare
        public ContentPageViewerModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Lista över alla användare (utom nuvarande)
        public List<IdentityUser> AllUsers { get; set; } = new();
        // Lista över alla profilbilder
        public List<PFP> AllPFPs { get; set; }
        // Lista över nuvarande användarens vänner
        public List<Friends> MyFriends { get; set; } = new();

        // Id för inlägget som visas (binds från querystring)
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        // Det inlägg som visas
        public Posts Post { get; set; }
        // Kommentarer till inlägget
        public List<Comments> PostComments { get; set; }

        // Text för ny kommentar (binds från formulär)
        [BindProperty]
        public string NewCommentText { get; set; }

        // Id för kommentar som ska tas bort (binds från formulär)
        [BindProperty]
        public int RemoveCommentId { get; set; }

        // POST: Flagga/avflagga en kommentar
        public async Task<IActionResult> OnPostToggleCommentFlaggedAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                return NotFound();

            comment.Flagged = !comment.Flagged; // Växla flaggad-status
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = Id }); // Använd bindade Id, inte Post?.Id
        }

        // GET: Ladda inlägg, kommentarer, användare, vänner och profilbilder
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            AllUsers = await Task.Run(() => _context.Users.Where(u => u.Id != currentUserId).ToList());
            MyFriends = await Task.Run(() => _context.Friends.Where(f => f.UserId == currentUserId).ToList());

            Post = await _context.Posts.FindAsync(id);
            if (Post == null)
                return NotFound();

            PostComments = _context.Comments.Where(c => c.PostsId == id).ToList();
            AllPFPs = await _context.PFPs.ToListAsync();
            return Page();
        }

        // POST: Lägg till en vän
        public async Task<IActionResult> OnPostAddFriendAsync(string friendUserId)
        {
            var currentUserId = _userManager.GetUserId(User);

            // Kontrollera om vänskapen redan finns
            var alreadyFriend = await Task.Run(() =>
                _context.Friends.Any(f => f.UserId == currentUserId && f.FriendUserId == friendUserId)
            );

            if (!alreadyFriend)
            {
                var friend = new Friends
                {
                    UserId = currentUserId,
                    FriendUserId = friendUserId
                };
                _context.Friends.Add(friend);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        // POST: Lägg till en kommentar till inlägget
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(NewCommentText))
                return await OnGetAsync(Id);

            var comment = new Comments
            {
                PostsId = Id,
                Text = NewCommentText,
                CreatedBy = User.Identity?.Name,
                CreatedAt = DateTime.UtcNow
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = Id });
        }
    }
}
