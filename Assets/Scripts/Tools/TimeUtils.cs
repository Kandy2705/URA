using System;

public static class TimeUtils
{
    /// <summary>
    /// Đổi giây -> (hours, minutes, seconds) dưới dạng int.
    /// Mặc định làm tròn xuống; set roundToNearestSecond = true để làm tròn gần nhất.
    /// </summary>
    public static (int hours, int minutes, int seconds)
        SecondsToHMS(double totalSeconds, bool roundToNearestSecond = false)
    {
        long total = (long)Math.Max(0, 
            roundToNearestSecond ? Math.Round(totalSeconds)
                : Math.Floor(totalSeconds));

        int hours   = (int)(total / 3600);
        int minutes = (int)((total % 3600) / 60);
        int seconds = (int)(total % 60);
        return (hours, minutes, seconds);
    }

    /// <summary>
    /// Bản out parameters (nếu bạn không dùng tuple).
    /// </summary>
    public static void SecondsToHMS(double totalSeconds,
        out int hours, out int minutes, out int seconds,
        bool roundToNearestSecond = false)
    {
        long total = (long)Math.Max(0, 
            roundToNearestSecond ? Math.Round(totalSeconds)
                : Math.Floor(totalSeconds));

        hours   = (int)(total / 3600);
        minutes = (int)((total % 3600) / 60);
        seconds = (int)(total % 60);
    }
}