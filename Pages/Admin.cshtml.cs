using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace No_Forum.Pages
{
    // Restricts access to users in the "Admin" role
    [Authorize(Roles = "Admin")]
    public class AdminModel : PageModel
    {
        // Provides user management functionality
        private readonly UserManager<IdentityUser> _userManager;

        // Constructor injects UserManager for user operations
        public AdminModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // List of all users except the current admin
        public List<IdentityUser> Users { get; set; } = new();

        // Stores lockout status for each user
        public Dictionary<string, bool> UserLockoutStatus { get; set; } = new();

        // Bound property for the user ID to ban
        [BindProperty]
        public string BanUserId { get; set; }

        // Bound property for the user ID to unban
        [BindProperty]
        public string UnbanUserId { get; set; }

        // Bound property for the user ID to remove
        [BindProperty]
        public string RemoveUserId { get; set; }

        // Loads users and their lockout status, excluding the current admin
        public async Task OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            Users = _userManager.Users
                .Where(u => u.Id != currentUser.Id)
                .ToList();

            foreach (var user in Users)
            {
                UserLockoutStatus[user.Id] = await _userManager.IsLockedOutAsync(user);
            }
        }

        // Bans a user by enabling lockout and setting lockout end date to max
        public async Task<IActionResult> OnPostBanAsync()
        {
            var user = await _userManager.FindByIdAsync(BanUserId);
            if (user != null)
            {
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            }
            return RedirectToPage();
        }

        // Unbans a user by clearing their lockout end date
        public async Task<IActionResult> OnPostUnbanAsync()
        {
            var user = await _userManager.FindByIdAsync(UnbanUserId);
            if (user != null)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
            }
            return RedirectToPage();
        }

        // Removes a user from the system
        public async Task<IActionResult> OnPostRemoveAsync()
        {
            var user = await _userManager.FindByIdAsync(RemoveUserId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToPage();
        }
    }
}
