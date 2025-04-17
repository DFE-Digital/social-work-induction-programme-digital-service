namespace Dfe.Sww.Ecf.Frontend.Extensions;

public static class EnumerableExtensions
{
    public static void AddRange<T>(this ICollection<T> collection, params T[] items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }
}
