using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using No_Forum.Data;
using No_Forum.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        [BindProperty(SupportsGet = true)]
        public List<string> Tags { get; set; } = new();

        public List<string> SelectedTags => Tags ?? new List<string>();

        public void OnGet()
        {
            var query = _context.Forumpages.AsQueryable();

            if (Tags != null && Tags.Count > 0)
            {
                foreach (var tag in Tags)
                {
                    switch (tag)
                    {
                        case "Political":
                            query = query.Where(f => f.Political);
                            break;
                        case "NSFW":
                            query = query.Where(f => f.NSFW);
                            break;
                        case "Roleplay":
                            query = query.Where(f => f.Roleplay);
                            break;
                        case "Discussion":
                            query = query.Where(f => f.Discussion);
                            break;
                        case "Meme":
                            query = query.Where(f => f.Meme);
                            break;
                        case "Art":
                            query = query.Where(f => f.Art);
                            break;
                        case "Technology":
                            query = query.Where(f => f.Technology);
                            break;
                    }
                }
            }

            ForumPages = query.ToList();
        }
    }
}
