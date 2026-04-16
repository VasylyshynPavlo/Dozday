namespace Dozday.Core.Interfaces;

public interface IAuthService
{
    Task<string?> LoginWithGoogleAsync(string IdToken, string? accessToken = null);
}