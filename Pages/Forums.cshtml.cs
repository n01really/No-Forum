using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using No_Forum.Data;
using No_Forum.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace No_Forum.Pages
{
    // PageModel för Forums-sidan
    public class ForumsModel : PageModel
    {
        // Databas-koppling via dependency injection
        private readonly ApplicationDbContext _context;

        // Konstruktor som tar in databaskontexten
        public ForumsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lista med forum-sidor som visas på sidan
        public List<Forumpages> ForumPages { get; set; } = new();

        // Lista med taggar som valts via query string (binds automatiskt)
        [BindProperty(SupportsGet = true)]
        public List<string> Tags { get; set; } = new();

        // Returnerar valda taggar, eller tom lista om inga finns
        public List<string> SelectedTags => Tags ?? new List<string>();

        // Körs vid GET-anrop till sidan
        public void OnGet()
        {
            // Startar en query mot alla forum-sidor
            var query = _context.Forumpages.AsQueryable();

            // Om taggar har valts, filtrera forum-sidor baserat på dessa
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

            // Hämtar ut de filtrerade forum-sidorna till listan
            ForumPages = query.ToList();
        }
    }
}
