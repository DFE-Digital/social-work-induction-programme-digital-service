using System.Text.Json;

namespace Dfe.Sww.Ecf.Frontend.Extensions;

public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static bool TryGet<T>(this ISession session, string key, out T? value)
    {
        var state = session.GetString(key);
        value = default;
        if (state == null)
            return false;
        value = JsonSerializer.Deserialize<T>(state);
        return true;
    }
}
