using System;
using System.Collections.Generic;


namespace SSO4Module
{
    public static class Timming
    {
    public static long GetUnixTime(DateTimeOffset dateTime)
    {
        return dateTime.ToUnixTimeMilliseconds();
    }

    public static long GetUnixTimeNow()
    {
        return GetUnixTime(GetUtcNow());
    }
    public static DateTimeOffset GetUtcNow()
    {
        return DateTimeOffset.UtcNow;
    }
   public static long GetUnixTimeForNextDay1AM(DateTimeOffset time)
    {
    TimeZoneInfo localTimeZone = TimeZoneInfo.Local; 
    DateTimeOffset localTime = TimeZoneInfo.ConvertTimeFromUtc(time.UtcDateTime, localTimeZone); 
    DateTimeOffset nextDay1AMLocal = localTime.Date.AddDays(1).AddHours(1);
    // Corrected conversion
    DateTimeOffset nextDay1AMUtc = TimeZoneInfo.ConvertTimeToUtc(nextDay1AMLocal.DateTime);
    return GetUnixTime(nextDay1AMUtc); 
    }
    }
}
 