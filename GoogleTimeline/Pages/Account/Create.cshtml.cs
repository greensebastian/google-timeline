using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccess;
using Microsoft.Extensions.Configuration;

namespace GoogleTimeline.Pages.Account
{
    public class CreateModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        public string Email { get; set; }
        public string ReturnUrl { get; set; }
        public bool RequireRegistrationKey { get; set; }

        public CreateModel(SignInManager<User> signInManager, UserManager<User> userManager, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        private string RegistrationSecret => _configuration.GetValue<string>("RegistrationKey");

        public async Task<IActionResult> OnGet(string returnUrl = null)
        {
            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if(externalLoginInfo == null)
            {
                return Redirect("/");
            }
            if(!externalLoginInfo.Principal.HasClaim(claim => claim.Type == ClaimTypes.Email))
            {
                return Redirect("/");
            }
            Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            ReturnUrl = returnUrl;
            RequireRegistrationKey = !string.IsNullOrEmpty(RegistrationSecret);
            return Page();
        }

        public async Task<IActionResult> OnPost(string returnUrl = null, string registrationKey = null)
        {
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null || !info.Principal.HasClaim(claim => claim.Type == ClaimTypes.Email))
            {
                throw new ApplicationException("Attempted to create user without external email info provided");
            }

            if (!string.IsNullOrEmpty(RegistrationSecret) && registrationKey != RegistrationSecret)
            {
                return BadRequest("The registration key is not valid");
            }

            Email = info.Principal.FindFirstValue(ClaimTypes.Email);

            var user = new User { UserName = Email, Email = Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    // Include the access token in the properties
                    var props = new AuthenticationProperties();
                    props.StoreTokens(info.AuthenticationTokens);

                    await _signInManager.SignInAsync(user, props, authenticationMethod: info.LoginProvider);

                    if (string.IsNullOrWhiteSpace(returnUrl))
                    {
                        return RedirectToPage("/Account");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
