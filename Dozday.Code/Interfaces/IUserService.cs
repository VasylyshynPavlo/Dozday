using Dozday.Core.DTOs;
using Dozday.Core.Models;
using System.Linq.Expressions;

namespace Dozday.Core.Interfaces;

public interface IUserService
{
    Task AddAsync(UserDto entity);
    Task AddRangeAsync(IEnumerable<UserDto> entities);

    Task<UserDto?> GetUserByIdAsync(string id);

    Task<PagedResult<TResult>> GetUsersAsync<TResult>(
        Expression<Func<User, TResult>> selector,
        Expression<Func<User, bool>> predicate,
        int page = 1, int pageSize = 10,
        Dictionary<string, bool>? orderBy = null);

    Task<PagedResult<(UserIdNameAvatarDto, bool sub)>> GetTeacherShortIfoWithSubStatusAsync(
        string currentUserId,
        int page = 1,
        int pageSize = 10,
        string? fullName = null);

    Task<int> DeleteUserByIdAsync(string id, string userId);
    Task<int> DeleteUserByEmailAsync(string email, string userId);

    Task UpdateUserByIdAsync(UserDto user);

    Task UnSubscribeUserAsync(string userId, string teacherId);
    Task SubscribeUserAsync(string userId, string teacherId);
    Task<int> UpdateUserByIdAsync(UserUpdateDto entity);

    Task<List<UserDto>> SearchAsync(string text);
    Task BanUserAsync(string userId, string adminId);
    Task UnBanUserAsync(string userId, string adminId);

    Task<bool> IsBannedAsync(string userId);
}