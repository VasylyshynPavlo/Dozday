using Dozday.Core.Enums;

namespace Dozday.Core.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; } = string.Empty;
    public UserRoles Role { get; set; } = UserRoles.None;
    public bool Banned { get; set; } = false;
    public List<Event> OrganizedEvents { get; set; } = new();
    public List<UserSubscription> UserSubscriptions { get; set; } = new();
    public List<UserEventSubscription> EventSubscriptions { get; set; } = new();
    public List<UserSubscription> Subscribers { get; set; } = new();
}