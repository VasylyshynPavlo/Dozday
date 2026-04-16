using Dozday.Core.Enums;
using Dozday.Core.Utils;

namespace Dozday.Core.Models;

public class Event
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Summary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Loacation { get; set; } = string.Empty;
    public DateTime Start { get; set; } = DateTime.Now;
    public DateTime End { get; set; }
    public Guid OrganizatorId { get; set; }
    public User Organizator { get; set; } = null!;
    public string? MeetingLink { get; set; } = null;
    public EventType Type { get; set; } = EventType.None;

    public string GetColorHex() => ColorUtils.RandomColorFromSeed(Id.ToString("N"));
}