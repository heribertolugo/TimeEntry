namespace TimeClock.Core.Helpers;
public static class PrimitiveTypesExtensions
{
    public static double? ToDoubleOrNull(this string? value)
    {
        double result;
        if (string.IsNullOrWhiteSpace(value) || !double.TryParse(value, out result))
            return null;
        return result;
    }
}
