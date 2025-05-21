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

        public ContentPageModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public Forumpages? ForumPage { get; set; }
        public List<Posts> ForumPosts { get; set; } = new();

        [BindProperty]
        public string NewPostText { get; set; } = string.Empty;

        public int ForumId { get; set; }
        public void OnGet(int id)
        {
            ForumId = id;
            ForumPage = _context.Forumpages.FirstOrDefault(f => f.Id == id);
            ForumPosts = _context.Posts.Where(p => p.ForumpageId == id).ToList(); // Corrected property name
        }
        public IActionResult OnPost(int id)
        {
            ForumId = id;
            if (!string.IsNullOrWhiteSpace(NewPostText))
            {
                var post = new Posts
                {
                    ForumpageId = id, // Corrected property name
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.Identity?.Name,
                    Text = NewPostText
                };
                _context.Posts.Add(post);
                _context.SaveChanges();
            }

            ForumPage = _context.Forumpages.FirstOrDefault(f => f.Id == id);
            ForumPosts = _context.Posts.Where(p => p.ForumpageId == id).ToList(); // Corrected property name
            NewPostText = string.Empty;
            return Page();
        }
    }
}
