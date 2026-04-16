using Dozday.Core.Interfaces;
using Dozday.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dozday.Data.Repositories;

public class UserRepository(DozdayDbContext context) : IUserRepository
{
    private readonly DozdayDbContext _context = context;

    public async Task AddAsync(User entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<User> entities)
    {
        await _context.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        return await _context.Users.Where(u => u.Id == id).ExecuteDeleteAsync();
    }

    public async Task<int> DeleteRangeByEmailAsync(string email)
    {
        return await _context.Users.Where(u => u.Email == email).ExecuteDeleteAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByNameAsync(string name)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.FullName == name);
    }

    public async Task<PagedResult<TResult>> GetUsersAsync<TResult>(
        Expression<Func<User, TResult>> selector,
        Expression<Func<User, bool>> predicate,
        int page = 1, int pageSize = 10,
        Dictionary<string, bool>? orderBy = null)
    {
        var query = _context.Users.AsNoTracking();

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

        var items = await query
            .Select(selector)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<TResult>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    public async Task<int> UpdateAsync(User entity)
    {
        var count = await _context.Users.Where(u => u.Id == entity.Id).ExecuteUpdateAsync(u => u
            .SetProperty(p => p.FullName, entity.FullName)
            .SetProperty(p => p.Email, entity.Email)
            .SetProperty(p => p.Role, entity.Role)
            .SetProperty(p => p.AvatarUrl, entity.AvatarUrl));
        await _context.SaveChangesAsync();
        return count;
    }
}