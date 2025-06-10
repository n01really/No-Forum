using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using No_Forum.Data;
using No_Forum.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace No_Forum.Pages
{
    // PageModel för sidan som hanterar rapporterade inlägg och kommentarer
    public class ReportedModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Konstruktor som tar in databascontext
        public ReportedModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lista med flaggade inlägg
        public List<Posts> FlaggedPosts { get; set; } = new();
        // Lista med flaggade kommentarer
        public List<Comments> FlaggedComments { get; set; } = new();

        // Hämtar alla flaggade inlägg och kommentarer vid sidladdning (GET)
        public async Task OnGetAsync()
        {
            FlaggedPosts = await _context.Posts.Where(p => p.Flagged).ToListAsync();
            FlaggedComments = await _context.Comments.Where(c => c.Flagged).ToListAsync();
        }

        // POST: Avflaggning av ett inlägg (tar bort flaggan)
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

        // POST: Tar bort ett inlägg permanent
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

        // POST: Avflaggning av en kommentar (tar bort flaggan)
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

        // POST: Tar bort en kommentar permanent
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