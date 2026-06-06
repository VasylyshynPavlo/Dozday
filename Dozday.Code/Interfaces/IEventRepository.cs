using Dozday.Core.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dozday.Core.Interfaces
{
    public interface IEventRepository
    {
        Task AddAsync(Event entity);

        Task AddRangeAsync(IEnumerable<Event> entities);

        // Get

        Task<Event?> GetByIdAsync(Guid id);

        Task<PagedResult<TResult>> GetEventAsync<TResult>(
        Expression<Func<Event, TResult>> selector,
        Expression<Func<Event, bool>> predicate,
        int page = 1, int pageSize = 10,
        Dictionary<string, bool>? orderBy = null);

        Task<IEnumerable<Event>> GetAllAsync();

        Task<IEnumerable<Event>> GetPagedListAsync(int page, int pageSize);

        Task<IEnumerable<Event>> GetRangeListAsync(DateTime from, DateTime to);

        // Update

        Task<int> UpdateAsync(Event entity);

        // Delete 

        Task<int> DeleteAsync(Guid id);

        Task<bool> IsOwnerOf(Guid userId, Guid eventId);

        Task<IEnumerable<Event>> SearchAsync(string text);
    }
}
