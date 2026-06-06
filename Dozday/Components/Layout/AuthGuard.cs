using Dozday.Core.Interfaces;
using Dozday.Services.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;

namespace Dozday.Components.Layout;

public class AuthGuard
{
    private readonly IJSRuntime _js;
    private readonly NavigationManager _nav;
    private readonly GlobalStorageService _storage;
    private readonly IUserService _userService;

    public AuthGuard(IJSRuntime js, NavigationManager nav, GlobalStorageService storage, IUserService userService)
    {
        _js = js;
        _nav = nav;
        _storage = storage;
        _userService = userService;
    }

    public async Task CheckAsync()
    {
        var token = await _js.InvokeAsync<string?>("auth.getToken");

        if (string.IsNullOrWhiteSpace(token) || IsExpired(token))
        {
            await _js.InvokeVoidAsync("sessionStorage.setItem", "auth_message", "Ваша сесія закінчилася. Будь ласка, увійдіть знову.");
            await Logout();
            return;
        }

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        if (jwt != null)
        {
            var userId = jwt.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
            bool banned = false;
            if (userId != null) banned = await _userService.IsBannedAsync(userId);
            if (banned)
            {
                await _js.InvokeVoidAsync("sessionStorage.setItem", "auth_message", "Ви були заблоковані адміністратором сайту.");
                await Logout();
                return;
            }
        }

        _storage.Token = token;
    }

    private bool IsExpired(string token)
    {
        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwt.ValidTo <= DateTime.UtcNow;
        }
        catch
        {
            return true;
        }
    }

    private async Task Logout()
    {
        _storage.Token = null;
        await _js.InvokeVoidAsync("auth.clearToken");
        _nav.NavigateTo("/auth", true);
    }
}