using Dozday.Core.Enums;
using Dozday.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dozday.Core.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; } = string.Empty;
        public bool Banned { get; set; } = false;
        public UserRoles Role { get; set; } = UserRoles.None;
    }
}
