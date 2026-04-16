using Dozday.Core.DTOs;
using Dozday.Core.Enums;
using Dozday.Core.Interfaces;
using Dozday.Core.Models;
using System.Linq.Expressions;

namespace Dozday.Services.Services;

public class UserService(IUserRepository userRepository, IUserSubscriptionRepository userSubscriptionRepository)
    : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserSubscriptionRepository _userSubscriptionRepository = userSubscriptionRepository;

    public async Task AddAsync(User entity)
    {
        await _userRepository.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<User> entities)
    {
        await _userRepository.AddRangeAsync(entities);
    }

    public async Task<int> DeleteUserByEmailAsync(string email)
    {
        return await _userRepository.DeleteRangeByEmailAsync(email);
    }

    public async Task<int> DeleteUserByIdAsync(string id)
    {
        return await _userRepository.DeleteAsync(Guid.Parse(id));
    }

    public async Task<PagedResult<TResult>> GetUsersAsync<TResult>(
        Expression<Func<User, TResult>> selector,
        Expression<Func<User, bool>> predicate,
        int page = 1, int pageSize = 10,
        Dictionary<string, bool>? orderBy = null)
    {
        return await _userRepository.GetUsersAsync(selector, predicate, page, pageSize, orderBy);
    }

    public async Task<PagedResult<(UserIdNameAvatarDto, bool sub)>> GetTeacherShortIfoWithSubStatusAsync(
        string currentUserId, int page = 1, int pageSize = 10, string? fullName = null)
    {
        var userGuid = Guid.Parse(currentUserId);
        var data = await _userRepository.GetUsersAsync(u => new { u.Id, u.FullName, u.AvatarUrl },
            u => u.Role.HasFlag(UserRoles.Teacher) && u.Id != userGuid && (string.IsNullOrEmpty(fullName) ||
                                                                           u.FullName.ToLower()
                                                                               .Contains(fullName.ToLower())), page,
            pageSize);
        var userSubscriptions = await _userSubscriptionRepository.GetByUserIdAsync(userGuid);

        var subscribedTeacherIds = userSubscriptions.Select(s => s.TeacherId).ToHashSet();

        var users = data.Items.Select(u => new UserIdNameAvatarDto
        {
            Id = u.Id.ToString(),
            FullName = u.FullName,
            AvatarUrl = u.AvatarUrl
        }).ToList();


        var itemsWithSub = users.Select(u => (u, subscribedTeacherIds.Contains(Guid.Parse(u.Id)))).ToList();

        return new PagedResult<(UserIdNameAvatarDto, bool sub)>
        {
            Items = itemsWithSub,
            TotalCount = data.TotalCount,
            PageSize = pageSize,
            Page = page,
            TotalPages = data.TotalPages
        };
        throw new NotImplementedException();
    }

    public Task<User?> GetUserByIdAsync(string id)
    {
        return _userRepository.GetByIdAsync(Guid.Parse(id));
    }

    public async Task UpdateUserByIdAsync(User user)
    {
        await _userRepository.UpdateAsync(user);
    }

    //public async Task UpdateUserByIdByFieldAsync(User entity)
    //{
    //    await _userRepository.UpdateAsync(entity);
    //}

    public async Task UnSubscribeUserAsync(string userId, string teacherId)
    {
        var userGuid = Guid.Parse(userId);
        var teacherGuid = Guid.Parse(teacherId);
        await _userSubscriptionRepository.DeleteByUserIdAndTeacherIdAsync(userGuid, teacherGuid);
    }

    public async Task SubscribeUserAsync(string userId, string teacherId)
    {
        var userGuid = Guid.Parse(userId);
        var teacherGuid = Guid.Parse(teacherId);
        await _userSubscriptionRepository.AddAsync(new UserSubscription
        {
            UserId = userGuid,
            TeacherId = teacherGuid,
            Date = DateTime.UtcNow
        });
    }

    public async Task<int> UpdateUserByIdAsync(UserUpdateDto entity)
    {
        var user = await _userRepository.GetByIdAsync(Guid.Parse(entity.Id));
        if (user == null) throw new KeyNotFoundException("Користувач не знайдений");
        var updateUser = new User
        {
            Id = Guid.Parse(entity.Id),
            FullName = entity.FullName ?? user.FullName,
            AvatarUrl = entity.AvatarUrl ?? user.AvatarUrl,
            Role = entity.Role ?? user.Role
        };

        return await _userRepository.UpdateAsync(updateUser);
    }
}