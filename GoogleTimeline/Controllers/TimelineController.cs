using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using GoogleTimelineUI.Models;
using GoogleTimelineUI.Services;
using Microsoft.AspNetCore.Mvc;
using DataAccess;
using Microsoft.EntityFrameworkCore.Internal;
using System.Globalization;

namespace GoogleTimelineUI.Controllers
{
    public class TimelineController : Controller
    {
        private readonly UserService _userService;
        private readonly TimelineRepository _timelineRepository;

        public TimelineController(UserService userService, TimelineRepository timelineRepository)
        {
            _userService = userService;
            _timelineRepository = timelineRepository;
        }

        public async Task<IActionResult> LocationsByCount()
        {
            var user = await _userService.CurrentUser();

            var locations = _timelineRepository
                .GetTimelineData(user)
                .LocationsByCount()
                .OrderByDescending(location => location.Value)
                .Select(entry => new LocationCountModel
                {
                    LocationId = entry.Key.Id,
                    LocationName = entry.Key.Name,
                    Address = entry.Key.Address,
                    VisitCount = entry.Value,
                    MapsLink = GoogleUtil.MapsLink(entry.Key.Latitude, entry.Key.Longitude)
                })
                .ToList();

            return View(locations);
        }

        public async Task<IActionResult> TravelMethodsByCount()
        {
            var user = await _userService.CurrentUser();

            var travelMethods = _timelineRepository
                .GetTimelineData(user)
                .TravelMethodsByCount()
                .Select(entry => new ActivityCountModel
                {
                    TravelMethod = entry.Key,
                    Count = entry.Value,
                    Unit = ""
                })
                .OrderByDescending(travelMethod => travelMethod.Count)
                .ToList();
            
            return View("~/Views/Timeline/TravelMethodsByUnit.cshtml", travelMethods);
        }

        public async Task<IActionResult> TravelMethodsByDistance()
        {
            var user = await _userService.CurrentUser();

            var units = new[]
            {
                "meters",
                "km",
                "kkm"
            };
            var unitScale = 1000;
            var scaleThreshold = 10;

            var travelMethods = _timelineRepository
                .GetTimelineData(user)
                .TravelMethodsByDistance()
                .Select(entry => new ActivityCountModel
                {
                    TravelMethod = entry.Key,
                    Count = entry.Value,
                    Unit = units[0]
                })
                .OrderByDescending(travelMethod => travelMethod.Count)
                .Select(entry =>
                {
                    var currentUnitIndex = Array.IndexOf(units, entry.Unit);
                    if (currentUnitIndex == -1) return entry;
                    var newEntry = new ActivityCountModel
                    {
                        Count = entry.Count,
                        TravelMethod = entry.TravelMethod,
                        Unit = entry.Unit
                    };
                    while (newEntry.Count/unitScale > scaleThreshold && currentUnitIndex < units.Length - 1)
                    {
                        newEntry.Count /= unitScale;
                        newEntry.Unit = units[++currentUnitIndex];
                    }
                    return newEntry;
                })
                .ToList();

            return View("~/Views/Timeline/TravelMethodsByUnit.cshtml", travelMethods);
        }
    }
}
