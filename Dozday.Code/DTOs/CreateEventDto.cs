using Dozday.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dozday.Core.DTOs
{
    public class CreateEventDto
    {
        public string Summary { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Location { get; set; } = string.Empty;
        public DateTime Start { get; set; } = DateTime.Now;
        public DateTime End { get; set; }
        public string OrganizatorId { get; set; } = string.Empty;
        public string? MeetingLink { get; set; } = null;
        public EventType Type { get; set; } = EventType.None;
    }
}
