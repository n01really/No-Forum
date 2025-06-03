using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace No_Forum.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public List<IdentityUser> Users { get; set; } = new();
        public Dictionary<string, bool> UserLockoutStatus { get; set; } = new();

        [BindProperty]
        public string BanUserId { get; set; }
        [BindProperty]
        public string UnbanUserId { get; set; }
        [BindProperty]
        public string RemoveUserId { get; set; }

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

        public async Task<IActionResult> OnPostUnbanAsync()
        {
            var user = await _userManager.FindByIdAsync(UnbanUserId);
            if (user != null)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
            }
            return RedirectToPage();
        }

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
