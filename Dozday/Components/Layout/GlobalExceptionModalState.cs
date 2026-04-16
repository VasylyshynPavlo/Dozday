namespace Dozday.Components.Layout;

public class GlobalExceptionModalState
{
    private const string DefaultUnhandledErrorMessage = "Внутріня помилка";

    private static readonly HashSet<Type> HandledExceptionTypes =
    [
        typeof(ArgumentException),
        typeof(InvalidOperationException),
        typeof(KeyNotFoundException),
        typeof(FormatException)
    ];

    private readonly List<GlobalExceptionModalItem> _items = [];

    public bool IsVisible => _items.Count > 0;
    public IReadOnlyList<GlobalExceptionModalItem> Items => _items;

    public event Action? OnChange;

    public bool TryShow(Exception exception)
    {
        var message = ShouldHandle(exception)
            ? string.IsNullOrWhiteSpace(exception.Message)
                ? "Сталася помилка. Спробуйте ще раз."
                : exception.Message
            : DefaultUnhandledErrorMessage;

        _items.Add(new GlobalExceptionModalItem(Guid.NewGuid(), message));
        OnChange?.Invoke();
        return true;
    }

    public void Hide(Guid modalId)
    {
        var removedCount = _items.RemoveAll(item => item.Id == modalId);
        if (removedCount == 0) return;

        OnChange?.Invoke();
    }

    private static bool ShouldHandle(Exception exception)
    {
        for (var current = exception; current != null; current = current.InnerException)
        {
            var currentType = current.GetType();
            if (HandledExceptionTypes.Any(type => type.IsAssignableFrom(currentType))) return true;
        }

        return false;
    }

    public sealed record GlobalExceptionModalItem(Guid Id, string Message);
}