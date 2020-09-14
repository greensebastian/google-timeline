using Domain.Interface;
using GoogleTimelineUI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace GoogleTimeline.Pages.Timeline
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ITimelineService _timelineService;
        private readonly UserService _userService;
        public IndexModel(ITimelineService timelineService, UserService userService)
        {
            _timelineService = timelineService;
            _userService = userService;
        }
        public int PlaceVisitCount { get; set; }
        public int ActivitySegmentCount { get; set; }
        public string PeriodString { get; set; }
        public async Task OnGet()
        {
            var timelineData = _timelineService.GetTimelineData(await _userService.CurrentUser());
            PlaceVisitCount = timelineData?.PlaceVisits.Count ?? 0;
            ActivitySegmentCount = timelineData?.ActivitySegments.Count ?? 0;
        }

        private string DateRangeFormat(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }
    }
}