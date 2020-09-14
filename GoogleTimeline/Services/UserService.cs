using DataAccess;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace GoogleTimelineUI.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public UserService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<User> CurrentUser()
        {
            if (_signInManager.Context.User != null)
            {
                return await _userManager.FindByNameAsync(_signInManager.Context.User.Identity.Name);
            }
            return null;
        }
    }
}
