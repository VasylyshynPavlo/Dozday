using System;
using System.Collections.Generic;
using System.Text;

namespace Dozday.Core.Models
{
    public class UserEventSubscriptionArchive
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } = Guid.Empty;
        public Guid EventId { get; set; } = Guid.Empty;
        public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
    }
}
