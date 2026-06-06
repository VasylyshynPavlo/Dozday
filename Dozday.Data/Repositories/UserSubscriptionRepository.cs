using Dozday.Core.Interfaces;
using Dozday.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Dozday.Data.Repositories;

public class UserSubscriptionRepository(IDbContextFactory<DozdayDbContext> contextFactory) : IUserSubscriptionRepository
{
    private readonly IDbContextFactory<DozdayDbContext> _contextFactory = contextFactory;

    public async Task AddAsync(UserSubscription entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.UserSubscriptions.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.UserSubscriptions.Where(e => e.Id == id).ExecuteDeleteAsync();
    }

    public async Task DeleteByTeacherIdAsync(Guid teacherId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.UserSubscriptions.Where(e => e.TeacherId == teacherId).ExecuteDeleteAsync();
    }

    public async Task DeleteByUserIdAndTeacherIdAsync(Guid userId, Guid teacherId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.UserSubscriptions.Where(e => e.UserId == userId && e.TeacherId == teacherId)
            .ExecuteDeleteAsync();
    }

    public async Task DeleteByUserIdAsync(Guid userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.UserSubscriptions.Where(e => e.UserId == userId).ExecuteDeleteAsync();
    }

    public async Task<IEnumerable<UserSubscription>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.UserSubscriptions.ToListAsync();
    }

    public async Task<UserSubscription?> GetByIdAsync(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.UserSubscriptions.FindAsync(id);
    }

    public async Task<List<UserSubscription>> GetByTeacherIdAsync(Guid teacherId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.UserSubscriptions.Where(e => e.TeacherId == teacherId).ToListAsync();
    }

    public async Task<List<UserSubscription>> GetByUserIdAsync(Guid userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.UserSubscriptions.Where(e => e.UserId == userId).ToListAsync();
    }

    public async Task UpdateAsync(UserSubscription entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.UserSubscriptions.Where(e => e.Id == entity.Id).ExecuteUpdateAsync(e => e
            .SetProperty(p => p.UserId, entity.UserId)
            .SetProperty(p => p.TeacherId, entity.TeacherId));
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
}