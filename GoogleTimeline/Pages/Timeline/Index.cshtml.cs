using Common;
using DataAccess;
using GoogleTimelineUI.Models;
using GoogleTimelineUI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleTimeline.Pages.Timeline
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly TimelineRepository _timelineRepository;
        private readonly UserService _userService;
        public IndexModel(TimelineRepository timelineRepository, UserService userService)
        {
            _timelineRepository = timelineRepository;
            _userService = userService;
        }
        public int PlaceVisitCount { get; set; }
        public int ActivitySegmentCount { get; set; }
        public string PeriodString { get; set; }
        public int? VisitCount { get; set; }
        public List<DateTime> VisitDays { get; set; }
        public string MapsLink { get; set; }
        public string LocationRadius { get; set; }
        public List<CollapsibleAsync> AsyncSections { get; set; } = new List<CollapsibleAsync>();
        public async Task OnGet()
        {
            var timelineData = _timelineRepository.GetTimelineData(await _userService.CurrentUser());
            if (timelineData == null)
            {
                return;
            }

            var startDate = DateTime.MinValue;
            var endDate = DateTime.MaxValue;
            if (DateTime.TryParse(Request.Query["start"], out var queryStart))
            {
                startDate = queryStart;
            }
            if (DateTime.TryParse(Request.Query["end"], out var queryEnd))
            {
                endDate = queryEnd;
            }

            timelineData = timelineData?.ForPeriod(startDate, endDate);

            PlaceVisitCount = timelineData?.PlaceVisits.Count ?? 0;
            ActivitySegmentCount = timelineData?.ActivitySegments.Count ?? 0;

            if (!string.IsNullOrWhiteSpace(Request.Query["lat"]) && !string.IsNullOrWhiteSpace(Request.Query["lng"]) && !string.IsNullOrWhiteSpace(Request.Query["r"]))
            {
                var lat = double.Parse(Request.Query["lat"], CultureInfo.InvariantCulture);
                var lng = double.Parse(Request.Query["lng"], CultureInfo.InvariantCulture);
                var r = int.Parse(Request.Query["r"], CultureInfo.InvariantCulture);

                var daysVisited = timelineData.DaysVisited(lat, lng, r)
                    .OrderBy(date => date.Date)
                    .ToList();
                VisitCount = daysVisited.Count;
                VisitDays = VisitCount == 0 ? null : daysVisited;
                MapsLink = GoogleUtil.MapsLink(lat, lng, 10);
                LocationRadius = string.Format("{0} km", (r / 1000.0).ToString("N", CultureInfo.InvariantCulture));
            }

            AsyncSections.Add(new CollapsibleAsync
            {
                Endpoint = Url.Action("locationsByCount", "timeline"),
                Id = "locationsByCount",
                Title = "Locations by number of visits"
            });

            AsyncSections.Add(new CollapsibleAsync
            {
                Endpoint = Url.Action("travelMethodsByCount", "timeline"),
                Id = "travelMethodsByCount",
                Title = "Travel methods by count"
            });

            AsyncSections.Add(new CollapsibleAsync
            {
                Endpoint = Url.Action("travelMethodsByDistance", "timeline"),
                Id = "travelMethodsByDistance",
                Title = "Travel methods by distance"
            });
        }
    }
}
