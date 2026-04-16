namespace Dozday.Core.DTOs;

public class UserIdNameAvatarDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; } = string.Empty;
}