using Dozday.Core.Enums;

namespace Dozday.Core.DTOs;

public class UserUpdateDto
{
    public string Id { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public UserRoles? Role { get; set; }
}