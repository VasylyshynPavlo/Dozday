using AutoMapper;
using Dozday.Core.DTOs;
using Dozday.Core.Interfaces;
using Dozday.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dozday.Services.Services
{
    public class UserEventSubscriptionService(IUserEventSubscriptionRepository repository, IMapper mapper) : IUserEventSubscriptionService
    {
        private readonly IUserEventSubscriptionRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task AddAsync(UserEventSubscriptionDto subscription)
        {
            await _repository.AddAsync(_mapper.Map<UserEventSubscription>(subscription));
        }

        public async Task DeleteAsync(string subscriptionId)
        {
            await _repository.DeleteAsync(Guid.Parse(subscriptionId));
        }

        public async Task<IEnumerable<UserEventSubscriptionDto>> GetAllAsync()
        {
            return _mapper.Map<IEnumerable<UserEventSubscriptionDto>>(await _repository.GetAllAsync());
        }

        public async Task<PagedResult<TResult>> GetAsync<TResult>(Expression<Func<UserEventSubscription, TResult>> selector, Expression<Func<UserEventSubscription, bool>> predicate, int page = 1, int pageSize = 10, Dictionary<string, bool>? orderBy = null)
        {
            return await _repository.GetAsync(selector, predicate, page, pageSize, orderBy);
        }

        public async Task<IEnumerable<UserEventSubscriptionDto>> GetByEventIdAsync(string eventId)
        {
            return _mapper.Map<IEnumerable<UserEventSubscriptionDto>>(await _repository.GetByEventIdAsync(Guid.Parse(eventId)));
        }

        public async Task<IEnumerable<UserEventSubscriptionDto>> GetByUserIdAsync(string userId)
        {
            return _mapper.Map<IEnumerable<UserEventSubscriptionDto>>(await _repository.GetByUserIdAsync(Guid.Parse(userId)));
        }

        public async Task<int> GetSubscribersByEventIdAsync(string eventId)
        {
            if (!Guid.TryParse(eventId, out Guid eventGuid))
            {
                throw new ArgumentException("Невірний формат ID події");
            }
            var subscribers = await _repository.GetAsync(e => e.Id, e => e.EventId == eventGuid, -1, -1);
            return subscribers.TotalCount;
        }

        public async Task<Dictionary<string, int>> GetByRangeWithUniqueEventId(IEnumerable<string> eventIds)
        {
            var guidEventIds = eventIds.Select(Guid.Parse).ToList();

            var subscriptions = await _repository.GetAsync(
                e => e,
                e => guidEventIds.Contains(e.EventId),
                -1,
                -1);

            return subscriptions.Items
                .GroupBy(s => s.EventId.ToString())
                .ToDictionary(
                    g => g.Key,
                    g => g.Count());
        }

        public async Task<bool> IsSubed(string eventId, string userId)
        {
            if (!Guid.TryParse(eventId, out Guid eventGuid) || !Guid.TryParse(userId, out Guid userGuid))
            {
                throw new ArgumentException("Невірний формат ID події або користувача");
            }
            var subscription = await _repository.GetAsync(
                s => s,
                s => s.EventId == eventGuid && s.UserId == userGuid,
                1,
                1);
            return subscription.Items.Any();
        }
    }
}
