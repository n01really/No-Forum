using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
        public void OnGet(int id)
        {
            ForumPage = _context.Forumpages.FirstOrDefault(f => f.Id == id);
        }
    }
}
