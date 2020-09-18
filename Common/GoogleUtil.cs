using System.Globalization;

namespace Common
{
    public static class GoogleUtil
    {
        public static string MapsLink(double lat, double lng, int? zoom = null)
        {
            var latText = lat.ToString(CultureInfo.InvariantCulture);
            var lngText = lng.ToString(CultureInfo.InvariantCulture);
            var link = string.Format("https://google.com/maps/place/{0},{1}", latText, lngText);
            if (zoom.HasValue)
            {
                link += string.Format("/@{0},{1},{2}z", latText, lngText, zoom.Value);
            }
            return link;
        }
    }
}
