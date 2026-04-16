using Dozday.Core.Interfaces;
using Dozday.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Dozday.Data.Repositories;

public class UserSubscriptionRepository(DozdayDbContext context) : IUserSubscriptionRepository
{
    private readonly DozdayDbContext _context = context;

    public async Task AddAsync(UserSubscription entity)
    {
        await _context.UserSubscriptions.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _context.UserSubscriptions.Where(e => e.Id == id).ExecuteDeleteAsync();
    }

    public async Task DeleteByTeacherIdAsync(Guid teacherId)
    {
        await _context.UserSubscriptions.Where(e => e.TeacherId == teacherId).ExecuteDeleteAsync();
    }

    public async Task DeleteByUserIdAndTeacherIdAsync(Guid userId, Guid teacherId)
    {
        await _context.UserSubscriptions.Where(e => e.UserId == userId && e.TeacherId == teacherId)
            .ExecuteDeleteAsync();
    }

    public async Task DeleteByUserIdAsync(Guid userId)
    {
        await _context.UserSubscriptions.Where(e => e.UserId == userId).ExecuteDeleteAsync();
    }

    public async Task<IEnumerable<UserSubscription>> GetAllAsync()
    {
        return await _context.UserSubscriptions.ToListAsync();
    }

    public async Task<UserSubscription?> GetByIdAsync(Guid id)
    {
        return await _context.UserSubscriptions.FindAsync(id);
    }

    public async Task<List<UserSubscription>> GetByTeacherIdAsync(Guid teacherId)
    {
        return await _context.UserSubscriptions.Where(e => e.TeacherId == teacherId).ToListAsync();
    }

    public async Task<List<UserSubscription>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserSubscriptions.Where(e => e.UserId == userId).ToListAsync();
    }

    public async Task UpdateAsync(UserSubscription entity)
    {
        await _context.UserSubscriptions.Where(e => e.Id == entity.Id).ExecuteUpdateAsync(e => e
            .SetProperty(p => p.UserId, entity.UserId)
            .SetProperty(p => p.TeacherId, entity.TeacherId)
            .SetProperty(p => p.Date, entity.Date));
    }
}