using Dozday.Core.Enums;

namespace Dozday.Core.Models.Archive;

public class UserArchived
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; } = string.Empty;
    public UserRoles Role { get; set; } = UserRoles.None;
    public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
}