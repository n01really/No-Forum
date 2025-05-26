using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using No_Forum.Models;
using No_Forum.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace No_Forum.Pages
{
    public class ContentPageViewerModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ContentPageViewerModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public Posts Post { get; set; } 
        public List<Comments> PostComments { get; set; } 

        [BindProperty]
        public string NewCommentText { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Post = await _context.Posts.FindAsync(id); 
            if (Post == null)
                return NotFound();

            PostComments = _context.Comments.Where(c => c.PostsId == id).ToList(); 
            return Page();
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
