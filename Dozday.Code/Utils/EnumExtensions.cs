namespace Dozday.Core.Utils;

public static class EnumExtensions
{
    public static int[] ToIntArray<T>(this T flags) where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .Where(m => Convert.ToUInt64(m) != 0 && flags.HasFlag(m))
            .Select(m => Convert.ToInt32(m))
            .ToArray();
    }
}