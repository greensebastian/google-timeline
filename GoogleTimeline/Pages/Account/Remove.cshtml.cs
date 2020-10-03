using System.Threading.Tasks;
using DataAccess;
using GoogleTimelineUI.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GoogleTimelineUI.Pages.Account
{
    public class RemoveModel : PageModel
    {
        private readonly UserService _userService;
        private readonly TimelineRepository _timelineRepository;

        public RemoveModel(UserService userService, TimelineRepository timelineRepository)
        {
            _userService = userService;
            _timelineRepository = timelineRepository;
        }
        public async Task OnGet()
        {
            var user = await _userService.CurrentUser();

            if (user == null)
            {
                Redirect("/");
            }
            else
            {
                _timelineRepository.RemoveTimelineData(user);
                await _userService.RemoveUser(user);
            }
        }
    }
}
