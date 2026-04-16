using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;

namespace Dozday.Components.Layout;

public class GlobalErrorBoundary(GlobalExceptionModalState exceptionModalState, NavigationManager navigationManager)
    : ErrorBoundary, IDisposable
{
    private readonly GlobalExceptionModalState _exceptionModalState = exceptionModalState;
    private readonly NavigationManager _navigationManager = navigationManager;

    public void Dispose()
    {
        _navigationManager.LocationChanged -= OnLocationChanged;
    }

    protected override void OnInitialized()
    {
        _navigationManager.LocationChanged += OnLocationChanged;
        base.OnInitialized();
    }

    protected override Task OnErrorAsync(Exception exception)
    {
        if (_exceptionModalState.TryShow(exception))
        {
            Recover();
            return Task.CompletedTask;
        }

        return base.OnErrorAsync(exception);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        Recover();
    }
}