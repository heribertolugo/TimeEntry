namespace TimeClock.Core;

public static class ObjectExtensions
{
    public static IDictionary<string, object> ToDictionary(this object obj)
    {
        ArgumentNullException.ThrowIfNull(nameof(obj));

        Dictionary<string, object> result = new Dictionary<string, object>();

        foreach (System.Reflection.PropertyInfo p in obj.GetType().GetProperties())
        {
            object? val = p.GetValue(obj);

            if (val == null) continue;

            result.Add(p.Name, val);
        }

        return result;
    }
}
