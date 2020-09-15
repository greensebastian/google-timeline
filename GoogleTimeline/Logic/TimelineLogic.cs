using Common;
using DataAccess;
using Domain.Interface;
using Model.Timeline.Data;
using Model.Timeline.External;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoogleTimelineUI.Logic
{
    public class TimelineLogic
    {
        private readonly ITimelineService _timelineService;

        public TimelineLogic(ITimelineService timelineService)
        {
            _timelineService = timelineService;
        }

        public TimelineData GetTimelineData(User user)
        {
            return _timelineService.GetTimelineData(user);
        }

        public static TimelineData GetTimelineDataForPeriod(TimelineData data, DateTime startDate, DateTime endDate)
        {
            (startDate, endDate) = DateUtil.Sorted(startDate, endDate);
            data.ActivitySegments = data.ActivitySegments.Where(segment => segment.EndDateTime > startDate && segment.StartDateTime < endDate).ToList();
            data.PlaceVisits = data.PlaceVisits.Where(visit => visit.EndDateTime > startDate && visit.StartDateTime < endDate).ToList();
            return data;
        }

        public static HashSet<DateTime> GetNumberOfDaysVisited(List<DbPlaceVisit> placeVisits, double centerLatitude, double centerLongitude, int meterRadius)
        {
            var daysVisited = new HashSet<DateTime>();
            foreach(var visit in placeVisits)
            {
                if (CoordinateUtil.SurfaceDistance(centerLatitude, centerLongitude, visit.CenterLat, visit.CenterLng) < meterRadius)
                {
                    daysVisited.UnionWith(DateUtil.DaysBetween(visit.StartDateTime, visit.EndDateTime));
                }
            }
            return daysVisited;
        }
    }
}
