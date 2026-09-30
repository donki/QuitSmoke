namespace QuitSmoke.Helpers;

/// <summary>Cuánto hace del último cigarro, corto: «&lt; 1 min», «25 min», «3h 5min», «2d 4h».</summary>
public static class TimeFormat
{
    public static string Elapsed(TimeSpan timeSpan)
    {
        if (timeSpan.TotalMinutes < 1)
            return "< 1 min";
        if (timeSpan.TotalHours < 1)
            return $"{(int)timeSpan.TotalMinutes} min";
        if (timeSpan.TotalDays < 1)
            return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}min";
        return $"{(int)timeSpan.TotalDays}d {timeSpan.Hours}h";
    }
}
