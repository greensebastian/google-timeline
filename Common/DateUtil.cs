using System;

namespace Common
{
    public static class DateUtil
    {
        public static (DateTime, DateTime) Sorted(DateTime t1, DateTime t2)
        {
            return t1 < t2 ? (t1, t2) : (t2, t1);
        }
    }
}
