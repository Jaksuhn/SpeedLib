using System.Globalization;

namespace SpeedLib.SpeedLib.Extensions
{
    public static class Strings
    {
        public static string ToTitleCase(this string s) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLower());

        public static string GetLast(this string source, int tail_length) => tail_length >= source.Length ? source : source[^tail_length..];
    }
}
