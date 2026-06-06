using Dozday.Core.Enums;
using Dozday.Core.Interfaces;
using Dozday.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dozday.Data.Repositories;

public class UserRepository(IDbContextFactory<DozdayDbContext> contextFactory) : IUserRepository
{
    private readonly IDbContextFactory<DozdayDbContext> _contextFactory = contextFactory;

    public async Task AddAsync(User entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<User> entities)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.AddRangeAsync(entities);
        await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.Where(u => u.Id == id).ExecuteDeleteAsync();
    }

    public async Task<int> DeleteRangeByEmailAsync(string email)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.Where(u => u.Email == email).ExecuteDeleteAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByNameAsync(string name)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.FirstOrDefaultAsync(u => u.FullName == name);
    }

    public async Task<PagedResult<TResult>> GetUsersAsync<TResult>(
        Expression<Func<User, TResult>> selector,
        Expression<Func<User, bool>> predicate,
        int page = 1, int pageSize = 10,
        Dictionary<string, bool>? orderBy = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Users.AsNoTracking();

        query = query.Where(predicate);

        var totalCount = await query.CountAsync();

        if (orderBy != null && orderBy.Any())
        {
            IOrderedQueryable<User>? ordered = null;
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
            query = query.OrderBy(u => u.FullName);
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

    public async Task<bool> IsAdmin(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Users.AsNoTracking().Where(u => u.Id == id);
        var item = await query.FirstOrDefaultAsync();
        return item?.Role.HasFlag(UserRoles.Administrator) ?? false;
    }

    public async Task<int> UpdateAsync(User entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var count = await context.Users.Where(u => u.Id == entity.Id).ExecuteUpdateAsync(u => u
            .SetProperty(p => p.FullName, entity.FullName)
            .SetProperty(p => p.Email, entity.Email)
            .SetProperty(p => p.Role, entity.Role)
            .SetProperty(p => p.AvatarUrl, entity.AvatarUrl)
            .SetProperty(p => p.Banned, entity.Banned));
        await context.SaveChangesAsync();
        return count;
    }

    public async Task<IEnumerable<User>> SearchAsync(string text)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users
            .Where(u => u.FullName.Contains(text) || u.Email.Contains(text) || u.Id.ToString().Contains(text))
            .ToListAsync();
    }
}