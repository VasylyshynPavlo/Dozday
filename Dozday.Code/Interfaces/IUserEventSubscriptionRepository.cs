using Dozday.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dozday.Core.Interfaces
{
    public interface IUserEventSubscriptionRepository
    {
        Task AddAsync(UserEventSubscription subscription);
        Task<IEnumerable<UserEventSubscription>> GetAllAsync();
        Task<IEnumerable<UserEventSubscription>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<UserEventSubscription>> GetByEventIdAsync(Guid eventId);
        Task<PagedResult<TResult>> GetAsync<TResult>(Expression<Func<UserEventSubscription, TResult>> selector,
        Expression<Func<UserEventSubscription, bool>> predicate,
        int page = 1, int pageSize = 10,
        Dictionary<string, bool>? orderBy = null);
        Task DeleteAsync (Guid subscriptionId);
    }
}
