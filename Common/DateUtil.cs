using System;
using System.Collections.Generic;

namespace Common
{
    public static class DateUtil
    {
        public static (DateTime, DateTime) Sorted(DateTime t1, DateTime t2)
        {
            return t1 < t2 ? (t1, t2) : (t2, t1);
        }

        public static IEnumerable<DateTime> DaysBetween(DateTime startDate, DateTime endDate)
        {
            (startDate, endDate) = Sorted(startDate, endDate);
            while(startDate.Date <= endDate.Date)
            {
                yield return startDate.Date;
                startDate = startDate.AddDays(1);
            }
        }
    }
}
