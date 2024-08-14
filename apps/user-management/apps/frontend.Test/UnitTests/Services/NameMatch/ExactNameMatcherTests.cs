using Dfe.Sww.Ecf.Frontend.Services.NameMatch;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.NameMatch;

public class ExactNameMatcherTests
{
    private readonly ExactNameMatcher _sut;

    public ExactNameMatcherTests()
    {
        _sut = new();
    }

    [Theory, MemberData(nameof(SuccessTestData))]
    public void WhenPassedMatchingNames_ReturnsPassingScore(string dfeFullName, string sweFullName)
    {
        //Act
        var result = _sut.MatchConfidence(dfeFullName, sweFullName);

        // Assert
        result.Should().Be(1);
    }

    [Theory, MemberData(nameof(UnsuccessfulTestData))]
    public void WhenPassedNotMatchingNames_ReturnsFailingScore(
        string dfeFullName,
        string sweFullName
    )
    {
        //Act
        var result = _sut.MatchConfidence(dfeFullName, sweFullName);

        // Assert
        result.Should().Be(0.0);
    }

    public static IEnumerable<object[]> SuccessTestData =>
        new List<object[]>
        {
            new object[] { "John Smith", "John Smith" },
            new object[] { "John", "John" },
            new object[] { "Smith", "Smith" },
        };

    public static IEnumerable<object[]> UnsuccessfulTestData =>
        new List<object[]>
        {
            new object[] { "John Smith", "Jane Doe" },
            new object[] { "Smith", "John Smith" },
            new object[] { "John", "John Smith" }
        };
}
