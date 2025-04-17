namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Extensions;

public static class EnumerableExtensions
{
    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return new Bogus.Randomizer().ArrayElement(source.ToArray());
    }
}
