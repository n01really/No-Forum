using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using No_Forum.Data;
using No_Forum.Models;
using No_Forum.Service;

namespace No_Forum;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Hämtar anslutningssträng och konfigurerar databascontext för SQL Server
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        // Konfigurerar Identity med roller och kräver bekräftade konton
        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        // Lägger till Razor Pages-stöd
        builder.Services.AddRazorPages();

        // Registrerar HTTP-klienter för externa API-tjänster
        builder.Services.AddHttpClient<PostsApiService>();
        builder.Services.AddHttpClient<ForumApiService>(client =>
        {
            client.BaseAddress = new Uri("https://noapi.azure-api.net");
        });

        // Registrerar e-posttjänst
        builder.Services.AddTransient<IEmailSender, EmailSender>();

        // Lägger till generell HTTP-klient
        builder.Services.AddHttpClient();

        var app = builder.Build();

        // Konfigurerar HTTP-pipelinen beroende på miljö (utveckling/produktion)
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        // Skapar scope för att hantera roller, användare och seed-data
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Skapar Admin-roll om den inte finns
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Skapar admin-användare om den inte finns och lägger till i Admin-roll
            var adminEmail = "n01reallyplays@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var createResult = await userManager.CreateAsync(adminUser, "0104134697Rich!"); // Använd starkt lösenord
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    // Loggar eventuella fel vid skapande av admin-användare
                    foreach (var error in createResult.Errors)
                    {
                        Console.WriteLine($"Error creating admin user: {error.Description}");
                    }
                }
            }
            else
            {
                // Säkerställer att användaren är i Admin-roll
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Lägger till ett första inlägg om databasen är tom
            if (!db.Posts.Any())
            {
                db.Posts.Add(new Posts
                {
                    ForumpageId = 1, // Ange giltigt ForumpageId
                    Text = "Welcome! This is the first post.",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = null,
                    ImagePath = null,
                    Flagged = false
                });
                db.SaveChanges();
            }
        }

        // Aktiverar HTTPS, statiska filer, routing och authorization
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        // Mappar Razor Pages endpoints
        app.MapRazorPages();

        // Startar applikationen
        app.Run();
    }
}
