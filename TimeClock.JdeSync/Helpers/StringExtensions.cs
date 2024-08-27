using System.Text;

namespace TimeClock.JdeSync.Helpers;
internal static class StringExtensions
{
    public static bool IsOrdinalLess(this string value, string? other)
    {
        return value.CompareTo(other) < 0;
    }

    public static bool IsOrdinalEqual(this string value, string? other)
    {
        return value.CompareTo(other) == 0;
    }

    public static bool IsOrdinalGreater(this string value, string? other)
    {
        return value.CompareTo(other) > 0;
    }
    public static bool IsOrdinalLessOrEqual(this string value, string? other)
    {
        return value.CompareTo(other) <= 0;
    }
    public static bool IsOrdinalGreaterOrEqual(this string value, string? other)
    {
        return value.CompareTo(other) >= 0;
    }
}

public static class StringBuilderExtensions
{
    public static void AppendTabbed(this StringBuilder stringBuilder, string? text = null)
    {
        stringBuilder.Append($"{text ?? string.Empty}\t");
    }
}