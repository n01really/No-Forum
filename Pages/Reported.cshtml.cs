using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using No_Forum.Data;
using No_Forum.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace No_Forum.Pages
{
    public class ReportedModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReportedModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Posts> FlaggedPosts { get; set; } = new();
        public List<Comments> FlaggedComments { get; set; } = new();

        public async Task OnGetAsync()
        {
            FlaggedPosts = await _context.Posts.Where(p => p.Flagged).ToListAsync();
            FlaggedComments = await _context.Comments.Where(c => c.Flagged).ToListAsync();
        }

        public async Task<IActionResult> OnPostDismissPostAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                post.Flagged = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemovePostAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDismissCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                comment.Flagged = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}