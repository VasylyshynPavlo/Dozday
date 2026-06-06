using AutoMapper;
using Dozday.Core.DTOs;
using Dozday.Core.Enums;
using Dozday.Core.Interfaces;
using Dozday.Core.Models;
using System.Linq.Expressions;

namespace Dozday.Services.Services;

public class UserService(IUserRepository userRepository, IUserSubscriptionRepository userSubscriptionRepository, IMapper mapper)
    : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserSubscriptionRepository _userSubscriptionRepository = userSubscriptionRepository;
    private readonly IMapper _mapper = mapper;

    public async Task AddAsync(UserDto entity)
    {
        await _userRepository.AddAsync(_mapper.Map<User>(entity));
    }

    public async Task AddRangeAsync(IEnumerable<UserDto> entities)
    {
        var users = entities.Select(_mapper.Map<User>);
        await _userRepository.AddRangeAsync(users);
    }

    public async Task<int> DeleteUserByEmailAsync(string email, string userId)
    {
        if (!Guid.TryParse(userId, out Guid parsedId))
        {
            throw new ArgumentException("Невірний формат ID користувача");
        }
        if (!await _userRepository.IsAdmin(parsedId))
        {
            throw new UnauthorizedAccessException("Ви не є адміністратором");
        }
        return await _userRepository.DeleteRangeByEmailAsync(email);
    }

    public async Task<int> DeleteUserByIdAsync(string id, string userId)
    {
        if (!Guid.TryParse(id, out Guid parsedId))
        {
            throw new ArgumentException("Невірний формат ID користувача");
        }
        if (!Guid.TryParse(userId, out Guid parsedUserId))
        {
            throw new ArgumentException("Невірний формат ID користувача");
        }
        if (!await _userRepository.IsAdmin(parsedUserId))
        {
            throw new UnauthorizedAccessException("Ви не є адміністратором");
        }
        return await _userRepository.DeleteAsync(parsedId);
    }

    public async Task<PagedResult<TResult>> GetUsersAsync<TResult>(
        Expression<Func<User, TResult>> selector,
        Expression<Func<User, bool>> predicate,
        int page = 1, int pageSize = 10,
        Dictionary<string, bool>? orderBy = null)
    {
        return await _userRepository.GetUsersAsync(selector, predicate, page, pageSize, orderBy);
    }

    public async Task<List<UserDto>> SearchAsync(string text)
    {
        return _mapper.Map<List<UserDto>>(await _userRepository.SearchAsync(text));
    }

    public async Task<PagedResult<(UserIdNameAvatarDto, bool sub)>> GetTeacherShortIfoWithSubStatusAsync(
        string currentUserId, int page = 1, int pageSize = 10, string? fullName = null)
    {
        if (!Guid.TryParse(currentUserId, out Guid userGuid))
        {
            throw new ArgumentException("Невірний формат ID користувача");
        }
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
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        if (!Guid.TryParse(id, out Guid parsedId))
        {
            throw new ArgumentException("Невірний формат ID користувача");
        }
        return _mapper.Map<UserDto?>(await _userRepository.GetByIdAsync(parsedId));
    }

    public async Task UpdateUserByIdAsync(UserDto user)
    {
        await _userRepository.UpdateAsync(_mapper.Map<User>(user));
    }

    //public async Task UpdateUserByIdByFieldAsync(User entity)
    //{
    //    await _userRepository.UpdateAsync(entity);
    //}

    public async Task UnSubscribeUserAsync(string userId, string teacherId)
    {
        if (!Guid.TryParse(userId, out Guid userGuid))
        {
            throw new ArgumentException("Невірний формат ID користувача");
        }
        if (!Guid.TryParse(teacherId, out Guid teacherGuid))
        {
            throw new ArgumentException("Невірний формат ID вчителя");
        }
        await _userSubscriptionRepository.DeleteByUserIdAndTeacherIdAsync(userGuid, teacherGuid);
    }

    public async Task SubscribeUserAsync(string userId, string teacherId)
    {
        if (!Guid.TryParse(userId, out Guid userGuid))
        {
            throw new ArgumentException("Невірний формат ID користувача");
        }
        if (!Guid.TryParse(teacherId, out Guid teacherGuid))
        {
            throw new ArgumentException("Невірний формат ID вчителя");
        }
        var user = await _userRepository.GetByIdAsync(userGuid);
        if (user == null)
        {
            throw new KeyNotFoundException("Користувач не знайдений");
        }

        var teacher = await _userRepository.GetByIdAsync(teacherGuid);
        if (teacher == null)
        {
            throw new KeyNotFoundException("Вчитель не знайдений");
        }
        await _userSubscriptionRepository.AddAsync(new UserSubscription
        {
            UserId = userGuid,
            TeacherId = teacherGuid,
        });
    }

    public async Task<int> UpdateUserByIdAsync(UserUpdateDto entity)
    {
        if (!Guid.TryParse(entity.Id, out Guid parsedId))
        {
            throw new ArgumentException("Невірний формат ID користувача");
        }
        var user = await _userRepository.GetByIdAsync(parsedId);
        if (user == null) throw new KeyNotFoundException("Користувач не знайдений");
        var updateUser = new User
        {
            Id = parsedId,
            FullName = entity.FullName ?? user.FullName,
            AvatarUrl = entity.AvatarUrl ?? user.AvatarUrl,
            Role = entity.Role ?? user.Role
        };

        return await _userRepository.UpdateAsync(updateUser);
    }

    public async Task BanUserAsync(string userId, string adminId)
    {
        if (!Guid.TryParse(userId, out Guid parsedId) || !Guid.TryParse(adminId, out Guid parsedUserId))
        {
            throw new ArgumentException("Невірний формат ID користувача або адміністратора");
        }
        var elements = (await _userRepository.GetUsersAsync(u => u.Role, u => u.Id == parsedUserId, -1, -1)).Items;
        if (!elements.Any() || !elements.First().HasFlag(UserRoles.Administrator))
        {
            throw new UnauthorizedAccessException("Ви не є адміністратором");
        }
        var user = await _userRepository.GetByIdAsync(parsedId);
        if (user == null)
        {
            throw new KeyNotFoundException("Користувач не знайдений");
        }
        if (user.Banned)
        {
            Console.WriteLine($"User {userId} is already banned.");
            return;
        }
        user.Banned = true;
        await _userRepository.UpdateAsync(user);
    }

    public async Task UnBanUserAsync(string userId, string adminId)
    {
        if (!Guid.TryParse(userId, out Guid parsedId) || !Guid.TryParse(adminId, out Guid parsedUserId))
        {
            throw new ArgumentException("Невірний формат ID користувача або адміністратора");
        }
        var elements = (await _userRepository.GetUsersAsync(u => u.Role, u => u.Id == parsedUserId, -1, -1)).Items;
        if (!elements.Any() || !elements.First().HasFlag(UserRoles.Administrator))
        {
            throw new UnauthorizedAccessException("Ви не є адміністратором");
        }
        var user = await _userRepository.GetByIdAsync(parsedId);
        if (user == null)
        {
            throw new KeyNotFoundException("Користувач не знайдений");
        }
        if (!user.Banned)
        {
            return;
        }
        user.Banned = false;
        await _userRepository.UpdateAsync(user);
    }

    public async Task<bool> IsBannedAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var parsedId))
            throw new ArgumentException("Невірний формат ID користувача");

        var result = (await _userRepository.GetUsersAsync(
            u => u.Banned,
            u => u.Id == parsedId,
            -1,
            -1
        )).Items;

        if (result.Count == 0)
            throw new KeyNotFoundException("Користувач не знайдений");

        return result[0];
    }
}