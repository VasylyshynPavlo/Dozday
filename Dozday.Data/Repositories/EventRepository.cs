using Dozday.Core.Interfaces;
using Dozday.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dozday.Data.Repositories;

public class EventRepository(IDbContextFactory<DozdayDbContext> contextFactory) : IEventRepository
{
    private readonly IDbContextFactory<DozdayDbContext> _contextFactory = contextFactory;

    // Add

    public async Task AddAsync(Event entity)
    {
        NormalizeEventDates(entity);
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Events.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Event> entities)
    {
        foreach (var entity in entities)
        {
            NormalizeEventDates(entity);
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Events.AddRange(entities);
        await context.SaveChangesAsync();
    }

    // Get

    public async Task<Event?> GetByIdAsync(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Events.FindAsync(id);
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Events.ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetPagedListAsync(int page, int pageSize)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Events
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetRangeListAsync(DateTime from, DateTime to)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Events
            .Where(e => e.Start >= from && e.End <= to)
            .ToListAsync();
    }

    // Update

    public async Task<int> UpdateAsync(Event entity)
    {
        NormalizeEventDates(entity);
        await using var context = await _contextFactory.CreateDbContextAsync();
        var trackedEntity = context.Events.Local.FirstOrDefault(e => e.Id == entity.Id);
        if (trackedEntity is not null)
        {
            context.Entry(trackedEntity).State = EntityState.Detached;
        }
        context.Events.Update(entity);
        return await context.SaveChangesAsync();
    }

    // Delete 

    public async Task<int> DeleteAsync(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Events
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync();
    }

    public async Task<int> DeleteRangeByUserIdAsync(Guid userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Events
            .Where(e => e.OrganizatorId == userId)
            .ExecuteDeleteAsync();
    }

    public async Task<int> DeleteRangeByEmailAsync(string email)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Events
            .Where(e => e.Organizator.Email == email)
            .ExecuteDeleteAsync();
    }

    private static void NormalizeEventDates(Event entity)
    {
        entity.Start = NormalizeDateTime(entity.Start);
        entity.End = NormalizeDateTime(entity.End);
    }

    private static DateTime NormalizeDateTime(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }

    public async Task<PagedResult<TResult>> GetEventAsync<TResult>(
        Expression<Func<Event, TResult>> selector,
        Expression<Func<Event, bool>> predicate,
        int page = 1, int pageSize = 10,
        Dictionary<string, bool>? orderBy = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Events.AsNoTracking();

        query = query.Where(predicate);

        var totalCount = await query.CountAsync();

        if (orderBy != null && orderBy.Any())
        {
            IOrderedQueryable<Event>? ordered = null;
            foreach (var order in orderBy)
            {
                var isFirst = ordered == null;
                if (isFirst)
                    ordered = order.Value
                        ? query.OrderBy(e => EF.Property<object>(e, order.Key))
                        : query.OrderByDescending(e => EF.Property<object>(e, order.Key));
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
        if (page < 0 && pageSize < 0)
        {
            items = await query
                .Select(selector)
                .ToListAsync();
        }
        else
        {
            items = await query
                .Select(selector)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

    public async Task<bool> IsOwnerOf(Guid userId, Guid eventId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Events.AnyAsync(u => (u.Id == eventId && u.OrganizatorId == userId));
    }

    public async Task<IEnumerable<Event>> SearchAsync(string text)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Events
            .Where(e => e.Summary.ToLower().Contains(text.ToLower()) || e.OrganizatorId.ToString().Contains(text) || e.Id.ToString().Contains(text))
            .ToListAsync();
    }
}