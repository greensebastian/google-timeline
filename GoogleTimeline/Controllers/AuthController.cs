using DataAccess.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace GoogleTimeline.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Entry point which triggers the start of the external login flow
        /// </summary>
        /// <param name="returnUrl">Url to return the user to once the whole flow is completed</param>
        public IActionResult Login(string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("callback", "auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        /// <summary>
        /// Sign out of the application
        /// </summary>
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }

        /// <summary>
        /// Handler which is triggered when the auth flow has completed, and the middleware has scaffolded the relevant information
        /// </summary>
        public async Task<IActionResult> Callback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                return StatusCode((int)HttpStatusCode.BadGateway, $"Error from external provider: {remoteError}");
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return Redirect(Url.Action("login", "auth"));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                // Store the access token and resign in so the token is included in the cookie
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                var props = new AuthenticationProperties();
                props.StoreTokens(info.AuthenticationTokens);
                await _signInManager.SignInAsync(user, props, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return Ok("Your account is locked out");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                return RedirectToPage("/Account/Create", new { returnUrl });
            }
        }
    }
}
