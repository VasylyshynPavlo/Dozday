namespace Dozday.Core.Models.Archive;

public class UserSubscriptionArchived
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid TeacherId { get; set; }
    public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
}