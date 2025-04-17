using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Dfe.Sww.Ecf.Frontend.Extensions;

public static class TempDataExtensions
{
    public static void Set<T>(this ITempDataDictionary tempData, string key, T value)
    {
        tempData[key] = JsonSerializer.Serialize(value);
    }

    public static T? Get<T>(this ITempDataDictionary tempData, string key)
    {
        var value = tempData[key];
        return value?.ToString() is null
            ? default
            : JsonSerializer.Deserialize<T>(value.ToString()!);
    }

    public static T? Peek<T>(this ITempDataDictionary tempData, string key)
    {
        var value = tempData.Peek(key);
        return value?.ToString() is null
            ? default
            : JsonSerializer.Deserialize<T>(value.ToString()!);
    }
}
