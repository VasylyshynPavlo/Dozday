using Dozday.Core.DTOs;
using Dozday.Core.Enums;
using Dozday.Core.Models;

namespace Dozday.Core.Utils;

public static class CalendarEventUtils
{
    public readonly record struct EventColorPalette(string BackgroundColor, string BorderColor, string TextColor);

    public static string GetEventTypeIcon(EventType type) => type switch
    {
        EventType.Online => "🌐",
        EventType.Offline => "📍",
        EventType.Hybrid => "🔀",
        _ => "📅"
    };

    public static string GetEventTypeLabel(EventType type) => type switch
    {
        EventType.Online => "Онлайн",
        EventType.Offline => "Очно",
        EventType.Hybrid => "Онлайн + Очно",
        _ => "None"
    };

    public static string GetEventTypeClass(EventType type) => type switch
    {
        EventType.Online => "calendar-month__event--online",
        EventType.Offline => "calendar-month__event--offline",
        EventType.Hybrid => "calendar-month__event--hybrid",
        _ => "calendar-month__event--default"
    };

    public static EventColorPalette GetEventColors(EventDto calendarEvent)
    {
        var baseHue = GetEventBaseHue(calendarEvent);
        return new EventColorPalette(
            $"hsl({baseHue} 85% 93%)",
            $"hsl({(baseHue + 18) % 360} 70% 45%)",
            $"hsl({(baseHue + 30) % 360} 70% 22%)");
    }

    public static string BuildEventColorStyle(EventDto calendarEvent)
    {
        var colors = GetEventColors(calendarEvent);
        return $"background-color: {colors.BackgroundColor}; border-left-color: {colors.BorderColor}; color: {colors.TextColor};";
    }

    public static string BuildEventDetailsStyle(EventDto calendarEvent)
    {
        var colors = GetEventColors(calendarEvent);
        return $"border-top: 6px solid {colors.BorderColor}; background-color: {colors.BackgroundColor}; color: {colors.TextColor};";
    }

    private static int GetEventBaseHue(EventDto calendarEvent)
    {
        var hash = calendarEvent.Id != string.Empty
            ? calendarEvent.Id.GetHashCode()
            : HashCode.Combine(calendarEvent.Summary, calendarEvent.Start, calendarEvent.End);

        if (hash == int.MinValue)
        {
            hash = int.MaxValue;
        }

        return Math.Abs(hash) % 360;
    }
}
