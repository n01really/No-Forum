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
    public class ContentPageViewerModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ContentPageViewerModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<IdentityUser> AllUsers { get; set; } = new();
        public List<PFP> AllPFPs { get; set; }
        public List<Friends> MyFriends { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public Posts Post { get; set; } 
        public List<Comments> PostComments { get; set; } 

        [BindProperty]
        public string NewCommentText { get; set; }

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
            return RedirectToPage(new { id = Post?.Id });
        }

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
        public async Task<IActionResult> OnPostAddFriendAsync(string friendUserId)
        {
            var currentUserId = _userManager.GetUserId(User);

            // Prevent duplicate friendships
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
