using System;
using System.Collections.Generic;
using System.Text;

namespace Dozday.Core.Models
{
    public class UserEventSubscription
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } = Guid.Empty;
        public Guid EventId { get; set; } = Guid.Empty;

        public User User { get; set; } = null!;
        public Event Event { get; set; } = null!;
    }
}
