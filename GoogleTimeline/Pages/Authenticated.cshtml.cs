using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GoogleTimeline.Pages
{
    [Authorize]
    public class AuthenticatedModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public AuthenticatedModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

       
        public void OnGet()
        {

        }
    }
}
