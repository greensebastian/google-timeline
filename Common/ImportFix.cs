namespace Common
{
    public static class ImportFix
    {
        public static long FixLatitude(long latitude)
        {
            return latitude <= 900000000 ? latitude : latitude - uint.MaxValue - 1;
        }

        public static long FixLongitude(long longitude)
        {
            return longitude <= 1800000000 ? longitude : longitude - uint.MaxValue - 1;
        }
    }
}
