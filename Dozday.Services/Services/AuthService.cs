using Dozday.Core.Enums;
using Dozday.Core.Interfaces;
using Dozday.Core.Models;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Dozday.Services.Services;

public class AuthService(
    IJwtService jwtService,
    IUserService userService,
    IOptions<GoogleAuthOptions> googleAuthOptions) : IAuthService
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly GoogleAuthOptions _googleAuthOptions = googleAuthOptions.Value;

    private readonly IJwtService _jwtService = jwtService;
    private readonly IUserService _userService = userService;

    public async Task<string?> LoginWithGoogleAsync(string IdToken, string? accessToken = null)
    {
        GoogleJsonWebSignature.Payload? payload = null;
        if (!string.IsNullOrWhiteSpace(IdToken))
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(IdToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid Google ID token.", ex);
            }

        var peopleProfile = await GetGooglePeopleProfileAsync(accessToken);

        var email = peopleProfile?.Email ?? payload?.Email;
        if (string.IsNullOrWhiteSpace(email))
            throw new Exception("Email not found in Google profile. Check Google OAuth scopes: openid email profile.");

        var user = (await _userService.GetUsersAsync(u => u, u => u.Email == email)).Items.FirstOrDefault();
        if (user == null)
        {
            var domain = email.Split('@').LastOrDefault()?.ToLower();
            if (string.IsNullOrEmpty(domain)) throw new ArgumentNullException("Email domain is invalid.");
            if (!_googleAuthOptions.WhiteListedEmailDomains.Contains(domain))
                throw new Exception("Email domain is not allowed.");

            var name = peopleProfile?.Name ?? payload?.Name ?? email.Split('@')[0];
            var pictureUrl = peopleProfile?.PhotoUrl ?? payload?.Picture;
            var newUser = new User
            {
                FullName = name,
                Email = email,
                AvatarUrl = pictureUrl,
                Role = UserRoles.None
            };
            await _userService.AddAsync(newUser);

            return _jwtService.GenerateToken(newUser);
        }

        user.FullName = peopleProfile?.Name ?? payload?.Name ?? user.FullName;
        user.AvatarUrl = peopleProfile?.PhotoUrl ?? payload?.Picture ?? user.AvatarUrl;
        await _userService.UpdateUserByIdAsync(user);

        return _jwtService.GenerateToken(user);
        throw new NotImplementedException();
    }

    private static async Task<GooglePeopleProfile?> GetGooglePeopleProfileAsync(string? accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken)) return null;

        using var httpClient = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v3/userinfo");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<GoogleUserInfoResponse>(json, _jsonOptions);
        return new GooglePeopleProfile(
            dto?.Name,
            dto?.Picture,
            dto?.Email);
    }

    private sealed record GooglePeopleProfile(string? Name, string? PhotoUrl, string? Email);

    private sealed class GoogleUserInfoResponse
    {
        public string? Name { get; set; }
        public string? Picture { get; set; }
        public string? Email { get; set; }
    }
}