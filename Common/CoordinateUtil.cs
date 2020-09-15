using System;

namespace Common
{
    public static class CoordinateUtil
    {
        private static readonly double e7 = Math.Pow(10, 7);
        private const double EarthRadiusKm = 6378.137;

        public static long FixLatitude(long latitude)
        {
            return latitude <= 900000000 ? latitude : latitude - uint.MaxValue - 1;
        }

        public static long FixLongitude(long longitude)
        {
            return longitude <= 1800000000 ? longitude : longitude - uint.MaxValue - 1;
        }

        public static double ToDegrees(long e7Coordinate)
        {
            return e7Coordinate / e7;
        }

        public static int SurfaceDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = lat2 * Math.PI / 180 - lat1 * Math.PI / 180;
            var dLon = lon2 * Math.PI / 180 - lon1 * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = EarthRadiusKm * c;
            return (int)Math.Round(d * 1000); // meters
        }
    }
}
