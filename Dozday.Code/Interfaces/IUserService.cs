using Dozday.Core.DTOs;
using Dozday.Core.Models;
using System.Linq.Expressions;

namespace Dozday.Core.Interfaces;

public interface IUserService
{
    Task AddAsync(User entity);
    Task AddRangeAsync(IEnumerable<User> entities);

    Task<User?> GetUserByIdAsync(string id);

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

    Task<int> DeleteUserByIdAsync(string id);
    Task<int> DeleteUserByEmailAsync(string email);

    Task UpdateUserByIdAsync(User user);

    Task UnSubscribeUserAsync(string userId, string teacherId);
    Task SubscribeUserAsync(string userId, string teacherId);
    Task<int> UpdateUserByIdAsync(UserUpdateDto entity);
}