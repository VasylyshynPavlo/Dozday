using System;
using System.Collections.Generic;
using System.Text;

namespace Dozday.Core.Utils
{
    public static class DateUtils
    {
        public static DateTime NormalizeDateTime(DateTime value)
        {
            return value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };
        }

        public static DateTime ConvertToTimeZone(DateTime value, TimeZoneInfo timeZone)
        {
            var utcValue = DateUtils.NormalizeDateTime(value);
            return TimeZoneInfo.ConvertTimeFromUtc(utcValue, timeZone);
        }

        public static TimeZoneInfo GetDisplayTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }

            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv");
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }

            return TimeZoneInfo.Local;
        }
    }
}
