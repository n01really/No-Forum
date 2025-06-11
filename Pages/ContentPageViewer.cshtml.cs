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
    // Modellklass f�r sidan ContentPageViewer
    public class ContentPageViewerModel : PageModel
    {
        // Databas- och anv�ndarhanterare
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Konstruktor som injicerar databas och anv�ndarhanterare
        public ContentPageViewerModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Lista �ver alla anv�ndare (utom nuvarande)
        public List<IdentityUser> AllUsers { get; set; } = new();
        // Lista �ver alla profilbilder
        public List<PFP> AllPFPs { get; set; }
        // Lista �ver nuvarande anv�ndarens v�nner
        public List<Friends> MyFriends { get; set; } = new();

        // Id f�r inl�gget som visas (binds fr�n querystring)
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        // Det inl�gg som visas
        public Posts Post { get; set; }
        // Kommentarer till inl�gget
        public List<Comments> PostComments { get; set; }

        // Text f�r ny kommentar (binds fr�n formul�r)
        [BindProperty]
        public string NewCommentText { get; set; }

        // Id f�r kommentar som ska tas bort (binds fr�n formul�r)
        [BindProperty]
        public int RemoveCommentId { get; set; }

        // POST: Flagga/avflagga en kommentar
        public async Task<IActionResult> OnPostToggleCommentFlaggedAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                return NotFound();

            comment.Flagged = !comment.Flagged; // V�xla flaggad-status
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = Id }); // Anv�nd bindade Id, inte Post?.Id
        }

        // GET: Ladda inl�gg, kommentarer, anv�ndare, v�nner och profilbilder
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

        // POST: L�gg till en v�n
        public async Task<IActionResult> OnPostAddFriendAsync(string friendUserId)
        {
            var currentUserId = _userManager.GetUserId(User);

            // Kontrollera om v�nskapen redan finns
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

        // POST: L�gg till en kommentar till inl�gget
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
