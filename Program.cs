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

        // H�mtar anslutningsstr�ng och konfigurerar databascontext f�r SQL Server
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        // Konfigurerar Identity med roller och kr�ver bekr�ftade konton
        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        // L�gger till Razor Pages-st�d
        builder.Services.AddRazorPages();

        // Registrerar HTTP-klienter f�r externa API-tj�nster
        builder.Services.AddHttpClient<PostsApiService>();
        builder.Services.AddHttpClient<ForumApiService>(client =>
        {
            client.BaseAddress = new Uri("https://noapi.azure-api.net");
        });

        // Registrerar e-posttj�nst
        builder.Services.AddTransient<IEmailSender, EmailSender>();

        // L�gger till generell HTTP-klient
        builder.Services.AddHttpClient();

        var app = builder.Build();

        // Konfigurerar HTTP-pipelinen beroende p� milj� (utveckling/produktion)
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        // Skapar scope f�r att hantera roller, anv�ndare och seed-data
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

            // Skapar admin-anv�ndare om den inte finns och l�gger till i Admin-roll
            var adminEmail = "n01reallyplays@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var createResult = await userManager.CreateAsync(adminUser, "0104134697Rich!"); // Anv�nd starkt l�senord
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    // Loggar eventuella fel vid skapande av admin-anv�ndare
                    foreach (var error in createResult.Errors)
                    {
                        Console.WriteLine($"Error creating admin user: {error.Description}");
                    }
                }
            }
            else
            {
                // S�kerst�ller att anv�ndaren �r i Admin-roll
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // L�gger till ett f�rsta inl�gg om databasen �r tom
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
