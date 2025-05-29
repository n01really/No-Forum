using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace No_Forum.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet <No_Forum.Models.Forumpages> Forumpages { get; set; }
    public DbSet<No_Forum.Models.Posts> Posts { get; set; }
    public DbSet<No_Forum.Models.Comments> Comments { get; set; }
    public DbSet<No_Forum.Models.DM> DM { get; set; }
}
