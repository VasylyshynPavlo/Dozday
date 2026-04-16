namespace Dozday.Core.Models;

public class UserSubscription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid TeacherId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public User Teacher { get; set; } = null!;
}