using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using No_Forum.Data;
using No_Forum.Models;
using System.Collections.Generic;
using System.Linq;

namespace No_Forum.Pages
{
    public class ForumsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ForumsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Forumpages> ForumPages { get; set; } = new();

        public void OnGet()
        {
            ForumPages = _context.Forumpages.ToList();
        }
    }
}
