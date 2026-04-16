using Dozday.Core.Models;

namespace Dozday.Core.Interfaces;

public interface IUserSubscriptionRepository
{
    Task AddAsync(UserSubscription entity);
    Task UpdateAsync(UserSubscription entity);
    Task DeleteAsync(Guid id);
    Task DeleteByUserIdAsync(Guid userId);
    Task DeleteByTeacherIdAsync(Guid teacherId);
    Task DeleteByUserIdAndTeacherIdAsync(Guid userId, Guid teacherId);
    Task<IEnumerable<UserSubscription>> GetAllAsync();
    Task<UserSubscription?> GetByIdAsync(Guid id);
    Task<List<UserSubscription>> GetByUserIdAsync(Guid userId);
    Task<List<UserSubscription>> GetByTeacherIdAsync(Guid teacherId);
}