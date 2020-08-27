using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace GoogleTimeline.Pages.Account
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public string Email { get; set; }

        public IndexModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnGet()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            Email = user.Email;
        }
    }
}
