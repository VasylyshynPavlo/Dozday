using Dozday.Core.Enums;
using Dozday.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dozday.Core.DTOs
{
    public class EventDto
    {
        public string Id { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Location { get; set; } = string.Empty;
        public DateTime Start { get; set; } = DateTime.Now;
        public DateTime End { get; set; }
        public string OrganizatorId { get; set; } = string.Empty;
        public string OrganizatorName { get; set; } = string.Empty;
        public string? MeetingLink { get; set; } = null;
        public int Subscribers { get; set; } = 0;
        public EventType Type { get; set; } = EventType.None;
    }
}
