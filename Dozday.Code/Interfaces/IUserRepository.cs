using Dozday.Core.Models;
using System.Linq.Expressions;

namespace Dozday.Core.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User entity);
    Task AddRangeAsync(IEnumerable<User> entities);

    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByNameAsync(string name);
    Task<User?> GetByEmailAsync(string email);

    Task<PagedResult<TResult>> GetUsersAsync<TResult>(
        Expression<Func<User, TResult>> selector,
        Expression<Func<User, bool>> predicate,
        int page = 1, int pageSize = 10,
        Dictionary<string, bool>? orderBy = null);

    Task<int> UpdateAsync(User entity);

    Task<int> DeleteAsync(Guid id);
    Task<int> DeleteRangeByEmailAsync(string email);

    Task<bool> IsAdmin(Guid id);
    Task<IEnumerable<User>> SearchAsync(string text);
}