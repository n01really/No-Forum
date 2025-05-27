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
        [BindProperty]
        public IFormFile? NewPostImage { get; set; }
        public void OnGet(int id)
        {
            ForumId = id;
            ForumPage = _context.Forumpages.FirstOrDefault(f => f.Id == id);
            ForumPosts = _context.Posts.Where(p => p.ForumpageId == id).ToList();
            

        }
        //public IActionResult OnPost(int id)
        //{
        //    ForumId = id;
        //    if (!string.IsNullOrWhiteSpace(NewPostText))
        //    {
        //        var post = new Posts
        //        {
        //            ForumpageId = id, 
        //            CreatedAt = DateTime.UtcNow,
        //            CreatedBy = User.Identity?.Name,
        //            Text = NewPostText
        //        };
        //        _context.Posts.Add(post);
        //        _context.SaveChanges();
        //    }

        //    ForumPage = _context.Forumpages.FirstOrDefault(f => f.Id == id);
        //    ForumPosts = _context.Posts.Where(p => p.ForumpageId == id).ToList(); // Corrected property name
        //    NewPostText = string.Empty;
        //    return Page();

        //}
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
                CreatedBy = User.Identity?.Name,
                ImagePath = imagePath
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = ForumId });
        }
       


    }
}
