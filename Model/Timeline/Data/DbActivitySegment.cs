using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Model.Timeline.External;

namespace Model.Timeline.Data
{
    public class DbActivitySegment
    {
        [Key]
        public int Id { get; set; }

        public virtual DbWaypoint StartWaypoint { get; set; }

        [ForeignKey("StartWaypoint")]
        public int? StartWaypointId { get; set; }

        public virtual DbWaypoint EndWaypoint { get; set; }

        [ForeignKey("EndWaypoint")]
        public int? EndWaypointId { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        [MaxLength(50)]
        [Column(TypeName ="nvarchar(50)")]
        public string Confidence { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string ActivityType { get; set; }
        public int Distance { get; set; }

        [ForeignKey("DbActivitySegmentId")]
        public virtual List<DbWaypoint> Waypoints { get; set; }

        [ForeignKey("DbActivitySegmentId")]
        public virtual List<DbLocationVisit> TransitLocationVisits { get; set; }

        public DbActivitySegment()
        {

        }

        public DbActivitySegment(Activitysegment activitysegment)
        {
            StartDateTime = Constants.epoch.AddSeconds(activitysegment.duration.startTimestampMs / 1000).AddMilliseconds(activitysegment.duration.startTimestampMs % 1000);
            EndDateTime = Constants.epoch.AddSeconds(activitysegment.duration.endTimestampMs / 1000).AddMilliseconds(activitysegment.duration.endTimestampMs % 1000);
            ActivityType = activitysegment.activityType;
            Confidence = activitysegment.confidence;
            Distance = activitysegment.distance;
            StartWaypoint = new DbWaypoint(activitysegment.startLocation);
            EndWaypoint = new DbWaypoint(activitysegment.endLocation);
            Waypoints = activitysegment.waypointPath?.waypoints.Select(wayPoint => new DbWaypoint(wayPoint)).ToList() ?? new List<DbWaypoint>();
            TransitLocationVisits = activitysegment.transitPath?.transitStops.Select(stop => new DbLocationVisit(stop)).ToList() ?? new List<DbLocationVisit>();
        }

        public override int GetHashCode()
        {
            var dateString = StartDateTime.ToString(CultureInfo.InvariantCulture) + EndDateTime.ToString(CultureInfo.InvariantCulture);
            return dateString.GetHashCode();
        }
    }
}
