using Common;
using DataAccess;
using Domain.Interface;
using Model.Timeline.Data;
using Model.Timeline.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public int GetNumberOfDaysVisited(TimelineData data, int centerLatitude, int centerLongitude, int meterRadius)
        {
            throw new NotImplementedException();
            var daysVisited = new HashSet<DateTime>();
            foreach(var visit in data.PlaceVisits)
            {
                //if (Math.Pow(centerLatitude - visit.CenterLatE7))
            }
        }
    }
}
