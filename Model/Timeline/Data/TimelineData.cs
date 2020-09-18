using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Model.Timeline.Data
{
    public class TimelineData
    {
        [Key]
        public int Id { get; set; }
        public virtual List<DbPlaceVisit> PlaceVisits { get; set; } = new List<DbPlaceVisit>();
        public virtual List<DbActivitySegment> ActivitySegments { get; set; } = new List<DbActivitySegment>();

        public TimelineData ForPeriod(DateTime startDate, DateTime endDate)
        {
            (startDate, endDate) = DateUtil.Sorted(startDate, endDate);
            return new TimelineData
            {
                Id = Id,
                ActivitySegments = ActivitySegments.Where(segment => segment.EndDateTime > startDate && segment.StartDateTime < endDate).ToList(),
                PlaceVisits = PlaceVisits.Where(visit => visit.EndDateTime > startDate && visit.StartDateTime < endDate).ToList()
            };
        }

        public Dictionary<DbLocation, int> LocationsByCount()
        {
            var locationCounts = new Dictionary<DbLocation, int>();
            foreach(var location in PlaceVisits.Select(visit => visit.LocationVisit.Location))
            {
                if (!locationCounts.ContainsKey(location)) locationCounts[location] = 0;
                locationCounts[location]++;
            }
            return locationCounts;
        }

        public HashSet<DateTime> DaysVisited(double centerLatitude, double centerLongitude, int meterRadius)
        {
            var daysVisited = new HashSet<DateTime>();
            foreach (var visit in PlaceVisits)
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
