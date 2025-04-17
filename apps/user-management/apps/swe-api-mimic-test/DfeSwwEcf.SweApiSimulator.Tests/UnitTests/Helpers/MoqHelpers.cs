using FluentAssertions;
using Moq;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Helpers;

// TODO Move into a nuget package so it can be used in other solutions?
internal class MoqHelpers
{
    internal static T ShouldBeEquivalentTo<T>(T expected)
    {
        bool Validate(T actual)
        {
            try
            {
                actual.Should().BeEquivalentTo(expected);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        return Match.Create<T>(Validate);
    }
}
