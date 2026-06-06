using Dozday.Core.DTOs;
using Dozday.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dozday.Core.Interfaces
{
    public interface IEventService
    {
        Task AddAsync(CreateEventDto entity);

        // Get

        Task<EventDto?> GetByIdAsync(string id);

        Task<PagedResult<TResult>> GetEventsAsync<TResult>(
        Expression<Func<Event, TResult>> selector,
        Expression<Func<Event, bool>> predicate,
        int page = 1, int pageSize = 10,
        Dictionary<string, bool>? orderBy = null);

        Task<IEnumerable<EventDto>> GetAllAsync();
        Task<PagedResult<EventDto>> GetAllByOrganiserIdAsync(string organiserId);

        Task<IEnumerable<EventDto>> GetPagedListAsync(int page, int pageSize);
        Task<IEnumerable<EventDto>> GetRangeListAsync(DateTime from, DateTime to);
        Task<PagedResult<EventDto>> GetEventsOnlyAsync(
        Expression<Func<Event, bool>> predicate,
        int page = 1,
        int pageSize = 10,
        Dictionary<string, bool>? orderBy = null);

        // Update

        Task UpdateAsync(EventDto entity, string userId);
        // Delete 

        Task DeleteAsync(string id, string userId);

        Task<List<Event>> SearchAsync(string text);
    }
}
