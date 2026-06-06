using Dozday.Core.DTOs;
using Dozday.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dozday.Core.Interfaces
{
    public interface IUserEventSubscriptionService
    {
        Task AddAsync(UserEventSubscriptionDto subscription);
        Task<IEnumerable<UserEventSubscriptionDto>> GetAllAsync();
        Task<IEnumerable<UserEventSubscriptionDto>> GetByUserIdAsync(string userId);
        Task<IEnumerable<UserEventSubscriptionDto>> GetByEventIdAsync(string eventId);
        Task<PagedResult<TResult>> GetAsync<TResult>(Expression<Func<UserEventSubscription, TResult>> selector,
        Expression<Func<UserEventSubscription, bool>> predicate,
        int page = 1, int pageSize = 10,
        Dictionary<string, bool>? orderBy = null);
        Task DeleteAsync(string subscriptionId);
        Task<int> GetSubscribersByEventIdAsync(string eventId);
        Task<Dictionary<string, int>> GetByRangeWithUniqueEventId(IEnumerable<string> eventIds);
        Task<bool> IsSubed(string eventId, string userId);
    }
}
