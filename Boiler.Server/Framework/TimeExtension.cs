namespace Boiler.Server.Framework;

public static class TimeExtension
{
    public static DateTime DateNow()
    {
        DateTime utcTime = DateTime.UtcNow;
        TimeZoneInfo pragueZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        DateTime pragueDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, pragueZone);
        return pragueDateTime;
    }

    public static DateTime PragueDateNowAsUtc()
    {
        DateTime utcNow = DateTime.UtcNow;
        TimeZoneInfo pragueZone;

        try
        {
            pragueZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Prague");
        }
        catch (TimeZoneNotFoundException)
        {
            pragueZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        }
        catch (Exception)
        {
            throw;
        }

        DateTime pragueTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, pragueZone);
        return new DateTime(pragueTime.Year, pragueTime.Month, pragueTime.Day, pragueTime.Hour, pragueTime.Minute, pragueTime.Second, DateTimeKind.Utc);
    }
}