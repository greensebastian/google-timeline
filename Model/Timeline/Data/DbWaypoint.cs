using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Model.Timeline.External;

namespace Model.Timeline.Data
{
    public class DbWaypoint
    {
        [Key]
        public int Id { get; set; }
        public long LatitudeE7 { get; set; }
        public long LongitudeE7 { get; set; }
        public int? DbActivitySegmentId { get; set; }

        public DbWaypoint()
        {

        }

        public DbWaypoint(Location location)
        {
            LatitudeE7 = location.latitudeE7;
            LongitudeE7 = location.longitudeE7;
        }

        public DbWaypoint(Waypoint location)
        {
            LatitudeE7 = location.latE7;
            LongitudeE7 = location.lngE7;
        }

        public override int GetHashCode()
        {
            return (LatitudeE7.ToString() + LongitudeE7.ToString()).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var waypoint = obj as DbWaypoint;
            return waypoint != null && waypoint.LatitudeE7 == LatitudeE7 && waypoint.LongitudeE7 == LongitudeE7;
        }
    }
}
