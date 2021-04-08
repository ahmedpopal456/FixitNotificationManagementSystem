using System;

namespace Fixit.Notification.Management.Lib.Builders.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool IsEarlierThan(DateTime date1, DateTime date2)
        {
            int result = DateTime.Compare(date1, date2);

            if (result < 0)
                return true;
            else
                return false;
        }

        public static DateTime ConvertUtcTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(unixTimeStamp).ToLocalTime();
            return dt;
        }

        public static DateTime AdjustDateTime(DateTime dt)
        {
            var timeSpan = new TimeSpan(dt.Hour, dt.Minute, dt.Second).Ticks;
            var newDt = new DateTime(timeSpan);
            return newDt;
        }
    }
}
