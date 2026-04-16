using System.Security.Cryptography;
using System.Text;

namespace Dozday.Core.Utils;

public static class ColorUtils
{
    public static string RandomColorFromSeed(string seed)
    {
        var normalizedSeed = seed ?? string.Empty;
        var bytes = Encoding.UTF8.GetBytes(normalizedSeed);
        var hashBytes = SHA256.HashData(bytes);

        var value = BitConverter.ToUInt32(hashBytes, 0);
        var hue = value % 360;

        const double saturation = 0.68;
        const double lightness = 0.52;

        var (r, g, b) = HslToRgb(hue, saturation, lightness);
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    private static (int R, int G, int B) HslToRgb(double hue, double saturation, double lightness)
    {
        var chroma = (1 - Math.Abs((2 * lightness) - 1)) * saturation;
        var x = chroma * (1 - Math.Abs((hue / 60d % 2) - 1));
        var m = lightness - chroma / 2;

        double r1;
        double g1;
        double b1;

        if (hue < 60)
        {
            r1 = chroma;
            g1 = x;
            b1 = 0;
        }
        else if (hue < 120)
        {
            r1 = x;
            g1 = chroma;
            b1 = 0;
        }
        else if (hue < 180)
        {
            r1 = 0;
            g1 = chroma;
            b1 = x;
        }
        else if (hue < 240)
        {
            r1 = 0;
            g1 = x;
            b1 = chroma;
        }
        else if (hue < 300)
        {
            r1 = x;
            g1 = 0;
            b1 = chroma;
        }
        else
        {
            r1 = chroma;
            g1 = 0;
            b1 = x;
        }

        var r = (int)Math.Round((r1 + m) * 255);
        var g = (int)Math.Round((g1 + m) * 255);
        var b = (int)Math.Round((b1 + m) * 255);

        return (Math.Clamp(r, 0, 255), Math.Clamp(g, 0, 255), Math.Clamp(b, 0, 255));
    }
}
