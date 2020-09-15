using System.Globalization;

namespace Common
{
    public static class GoogleUtil
    {
        public static string MapsLink(double lat, double lng, int zoom)
        {
            var latText = lat.ToString(CultureInfo.InvariantCulture);
            var lngText = lng.ToString(CultureInfo.InvariantCulture);
            return string.Format("https://google.com/maps/place/{0},{1}/@{0},{1},{2}z", latText, lngText, zoom);
        }
    }
}
