using Dozday.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Dozday.Data.Repositories;

public class EventRepository(DozdayDbContext context)
{
    private readonly DozdayDbContext _context = context;

    // Add

    public async Task AddAsync(Event entity)
    {
        _context.Events.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Event> entities)
    {
        _context.Events.AddRange(entities);
        await _context.SaveChangesAsync();
    }

    // Get

    public async Task<Event?> GetByIdAsync(Guid id)
    {
        return await _context.Events.FindAsync(id);
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await _context.Events.ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetPagedListAsync(int page, int pageSize)
    {
        return await _context.Events
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetRangeListAsync(DateTime from, DateTime to)
    {
        return await _context.Events
            .Where(e => e.Start >= from && e.End <= to)
            .ToListAsync();
    }

    // Update

    public async Task<int> UpdateAsync(Event entity)
    {
        _context.Events.Update(entity);
        return await _context.SaveChangesAsync();
    }

    // Delete 

    public async Task<int> DeleteAsync(Guid id)
    {
        return await _context.Events
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync();
    }

    public async Task<int> DeleteRangeByUserIdAsync(Guid userId)
    {
        return await _context.Events
            .Where(e => e.OrganizatorId == userId)
            .ExecuteDeleteAsync();
    }

    public async Task<int> DeleteRangeByEmailAsync(string email)
    {
        return await _context.Events
            .Where(e => e.Organizator.Email == email)
            .ExecuteDeleteAsync();
    }
}