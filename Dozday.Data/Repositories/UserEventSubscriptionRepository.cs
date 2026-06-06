using Dozday.Core.Interfaces;
using Dozday.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dozday.Data.Repositories
{
    public class UserEventSubscriptionRepository(IDbContextFactory<DozdayDbContext> contextFactory) : IUserEventSubscriptionRepository
    {
        private readonly IDbContextFactory<DozdayDbContext> _contextFactory = contextFactory;

        public async Task AddAsync(UserEventSubscription subscription)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            await context.AddAsync(subscription);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            await context.UserEventSubscriptions.Where(s => s.Id == id).ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<UserEventSubscription>> GetAllAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.UserEventSubscriptions.ToListAsync();
        }

        public async Task<IEnumerable<UserEventSubscription>> GetByEventIdAsync(Guid eventId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.UserEventSubscriptions.Where(s => s.EventId == eventId).ToListAsync();
        }

        public async Task<PagedResult<TResult>> GetAsync<TResult>(Expression<Func<UserEventSubscription, TResult>> selector,
        Expression<Func<UserEventSubscription, bool>> predicate,
        int page = 1, int pageSize = 10,
        Dictionary<string, bool>? orderBy = null)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.UserEventSubscriptions.AsNoTracking();

            query = query.Where(predicate);

            var totalCount = await query.CountAsync();

            if (orderBy != null && orderBy.Any())
            {
                IOrderedQueryable<UserEventSubscription>? ordered = null;
                foreach (var order in orderBy)
                {
                    var isFirst = ordered == null;
                    if (isFirst)
                        ordered = order.Value
                            ? query.OrderBy(u => EF.Property<object>(u, order.Key))
                            : query.OrderByDescending(u => EF.Property<object>(u, order.Key));
                    else
                        ordered = order.Value
                            ? ordered!.ThenBy(u => EF.Property<object>(u, order.Key))
                            : ordered!.ThenByDescending(u => EF.Property<object>(u, order.Key));
                }

                query = ordered ?? query;
            }
            else
            {
                query = query.OrderBy(u => u.Id);
            }

            List<TResult> items;
            if (page == -1 && pageSize == -1)
            {
                items = await query
                    .Select(selector)
                    .ToListAsync();
            }
            else
            {
                items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(selector)
                    .ToListAsync();
            }

            return new PagedResult<TResult>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<IEnumerable<UserEventSubscription>> GetByUserIdAsync(Guid userId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.UserEventSubscriptions.Where(s => s.UserId == userId).ToListAsync();
        }
    }
}
