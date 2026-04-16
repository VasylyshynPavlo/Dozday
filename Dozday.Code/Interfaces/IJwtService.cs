using Dozday.Core.Models;

namespace Dozday.Core.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}