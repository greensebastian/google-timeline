using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Domain.Interface;
using GoogleTimelineUI.Models;
using GoogleTimelineUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoogleTimelineUI.Controllers
{
    public class TimelineController : Controller
    {
        private readonly UserService _userService;
        private readonly ITimelineService _timelineService;

        public TimelineController(UserService userService, ITimelineService timelineService)
        {
            _userService = userService;
            _timelineService = timelineService;
        }

        public async Task<IActionResult> LocationsByCount()
        {
            var user = await _userService.CurrentUser();
            var timelineData = _timelineService.GetTimelineData(user);

            var locations = timelineData.LocationsByCount()
                .OrderByDescending(location => location.Value)
                .Select(entry => new LocationCountModel
            {
                LocationId = entry.Key.Id,
                LocationName = entry.Key.Name,
                Address = entry.Key.Address,
                VisitCount = entry.Value,
                MapsLink = GoogleUtil.MapsLink(entry.Key.Latitude, entry.Key.Longitude)
            }).ToList();

            return View(locations);
        }
    }
}
