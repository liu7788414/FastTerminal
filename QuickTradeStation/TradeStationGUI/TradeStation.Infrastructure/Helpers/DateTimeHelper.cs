using System;

namespace TradeStation.Infrastructure.Extensions
{
    public static class DateTimeHelper
    {
        public static DateTime ConvertToDate(long dateInt)
        {
            return ConvertToDateTime(dateInt * 1000000000);
        }

        public static DateTime ConvertToDateTime(long dateTimeInt)
        {
            return ConvertToDateTime((int)(dateTimeInt / 1000000000), (int)(dateTimeInt % 1000000000));
        }

        public static DateTime ConvertToDateTime(Int32 dateInt, Int32 timeInt)
        {
            if (dateInt == 0 && timeInt == 0)
            {
                return DateTime.MinValue;
            }

            if (dateInt == 0)
            {
                // If the date is empty, set the date to the min value.
                dateInt += 10101; // 0001-01-01
            }

            int hour = timeInt / 10000000;
            int minute = (timeInt - hour * 10000000) / 100000;
            int second = (timeInt - hour * 10000000 - minute * 100000) / 1000;
            int millisecond = timeInt - hour * 10000000 - minute * 100000 - second * 1000;

            int year = dateInt / 10000;
            int month = (dateInt - year * 10000) / 100;
            int day = dateInt - year * 10000 - month * 100;

            return new DateTime(year, month, day, hour, minute, second, millisecond);
        }

        public static long ConvertToDateTimeInt(DateTime dateTime)
        {
            long result = 0;

            result += dateTime.Minute;
            result += dateTime.Hour * (long)Math.Pow(100, 1);
            result += dateTime.Day * (long)Math.Pow(100, 2);
            result += dateTime.Month * (long)Math.Pow(100, 3);
            result += dateTime.Year * (long)Math.Pow(100, 4);

            return result;
        }
    }
}
