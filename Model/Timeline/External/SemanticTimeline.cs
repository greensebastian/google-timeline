using Common;
using System.Collections.Generic;

namespace Model.Timeline.External
{
    public class SemanticTimeline
    {
        public List<Timelineobject> timelineObjects { get; set; }
    }

    public class Timelineobject
    {
        public Activitysegment activitySegment { get; set; }
        public Placevisit placeVisit { get; set; }
    }

    public class Activitysegment
    {
        public Location startLocation { get; set; }
        public Location endLocation { get; set; }
        public Duration duration { get; set; }
        public int distance { get; set; }
        public string activityType { get; set; }
        public string confidence { get; set; }
        public Activity[] activities { get; set; }
        public Waypointpath waypointPath { get; set; }
        public Simplifiedrawpath simplifiedRawPath { get; set; }
        public Transitpath transitPath { get; set; }
    }

    public class Duration
    {
        public long startTimestampMs { get; set; }
        public long endTimestampMs { get; set; }
    }

    public class Waypointpath
    {
        public Waypoint[] waypoints { get; set; }
    }

    public class Waypoint
    {
        private long _latE7;
        private long _lngE7;
        public long latE7 { get => ImportFix.FixLatitude(_latE7) ; set => _latE7 = value; }
        public long lngE7 { get => ImportFix.FixLongitude(_lngE7); set => _lngE7 = value; }
    }

    public class Simplifiedrawpath
    {
        public Point[] points { get; set; }
    }

    public class Point
    {
        private long _latE7;
        private long _lngE7;
        public long latE7 { get => ImportFix.FixLatitude(_latE7); set => _latE7 = value; }
        public long lngE7 { get => ImportFix.FixLongitude(_lngE7); set => _lngE7 = value; }
        public string timestampMs { get; set; }
        public int accuracyMeters { get; set; }
    }

    public class Transitpath
    {
        public Transitstop[] transitStops { get; set; }
        public string name { get; set; }
        public string hexRgbColor { get; set; }
    }

    public class Transitstop
    {
        private long _latE7;
        private long _lngE7;
        public long latitudeE7 { get => ImportFix.FixLatitude(_latE7); set => _latE7 = value; }
        public long longitudeE7 { get => ImportFix.FixLongitude(_lngE7); set => _lngE7 = value; }
        public string placeId { get; set; }
        public string name { get; set; }
    }

    public class Activity
    {
        public string activityType { get; set; }
        public float probability { get; set; }
    }

    public class Placevisit
    {
        public Location location { get; set; }
        public Duration duration { get; set; }
        public string placeConfidence { get; set; }
        public long centerLatE7 { get; set; }
        public long centerLngE7 { get; set; }
        public Childvisit[] childVisits { get; set; }
    }

    public class Location
    {
        private long _latE7;
        private long _lngE7;
        public long latitudeE7 { get => ImportFix.FixLatitude(_latE7); set => _latE7 = value; }
        public long longitudeE7 { get => ImportFix.FixLongitude(_lngE7); set => _lngE7 = value; }
        public string placeId { get; set; }
        public string address { get; set; }
        public string name { get; set; }
        public string semanticType { get; set; }
        public Sourceinfo sourceInfo { get; set; }
    }

    public class Sourceinfo
    {
        public int deviceTag { get; set; }
    }

    public class Childvisit
    {
        private long _latE7;
        private long _lngE7;
        public long centerLatE7 { get => ImportFix.FixLatitude(_latE7); set => _latE7 = value; }
        public long centerLngE7 { get => ImportFix.FixLongitude(_lngE7); set => _lngE7 = value; }
        public Location location { get; set; }
        public Duration duration { get; set; }
        public string placeConfidence { get; set; }
    }
}
