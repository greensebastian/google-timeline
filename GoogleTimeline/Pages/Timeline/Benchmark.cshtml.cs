using System;
using System.Net;
using System.Threading.Tasks;
using DataAccess;
using Domain.Interface;
using GoogleTimelineUI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GoogleTimelineUI.Pages.Timeline
{
    [Authorize]
    public class BenchmarkModel : PageModel
    {
        private readonly UserService _userService;
        private readonly ITimelineService _timelineService;

        public BenchmarkModel(UserService userService, ITimelineService timelineService)
        {
            _userService = userService;
            _timelineService = timelineService;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnGetRun(string type)
        {
            if(Enum.TryParse(type, true, out RetrievalMethod retrievalMethod)){
                var user = await _userService.CurrentUser();
                return Content(_timelineService.BenchmarkGetTimelineData(user, retrievalMethod).ToString());
            }
            return StatusCode((int)HttpStatusCode.BadRequest, $"Requires parameter \"type\" with one of the following values: [{string.Join(", ", Enum.GetNames(typeof(RetrievalMethod)))}]");
        }
    }
}
