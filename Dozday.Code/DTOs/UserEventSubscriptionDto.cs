using Dozday.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dozday.Core.DTOs
{
    public class UserEventSubscriptionDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
    }
}
