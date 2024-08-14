using Dfe.Sww.Ecf.Frontend.Services.NameMatch;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.NameMatch;

public class LevensteinMatcherTests
{
    private readonly LevensteinMatcher _sut;

    public LevensteinMatcherTests()
    {
        _sut = new();
    }

    [Theory, MemberData(nameof(ExactMatchTestData))]
    public void WhenPassedExactMatchingNames_ReturnsExpectedScore(
        string dfeFullName,
        string sweFullName
    )
    {
        // Arrange
        var exactScore = 1;

        //Act
        var result = _sut.MatchConfidence(dfeFullName, sweFullName);

        // Assert
        result.Should().Be(exactScore);
    }

    [Theory, MemberData(nameof(VeryGoodMatchTestData))]
    public void WhenPassedVeryGoodNames_ReturnsExpectedScore(string dfeFullName, string sweFullName)
    {
        // Arrange
        var exactMatchScore = 1;
        var passingScore = 0.75;

        //Act
        var result = _sut.MatchConfidence(dfeFullName, sweFullName);

        // Assert
        result.Should().BeLessThan(exactMatchScore);
        result.Should().BeGreaterThanOrEqualTo(passingScore);
    }

    [Theory, MemberData(nameof(GoodTestData))]
    public void WhenPassedGoodNames_ReturnsExpectedScore(string dfeFullName, string sweFullName)
    {
        // Arrange
        var veryGoodScore = 0.75;
        var goodScore = 0.5;

        //Act
        var result = _sut.MatchConfidence(dfeFullName, sweFullName);

        // Assert
        result.Should().BeLessThan(veryGoodScore);
        result.Should().BeGreaterThanOrEqualTo(goodScore);
    }

    [Theory, MemberData(nameof(AverageTestData))]
    public void WhenPassedAverageNames_ReturnsExpectedScore(string dfeFullName, string sweFullName)
    {
        // Arrange
        var goodScore = 0.5;
        var averageScore = 0.25;

        //Act
        var result = _sut.MatchConfidence(dfeFullName, sweFullName);

        // Assert
        result.Should().BeLessThan(goodScore);
        result.Should().BeGreaterThanOrEqualTo(averageScore);
    }

    [Theory, MemberData(nameof(PoorTestData))]
    public void WhenPassedPoorNames_ReturnsExpectedScore(string dfeFullName, string sweFullName)
    {
        // Arrange
        var poorScore = 0.25;

        //Act
        var result = _sut.MatchConfidence(dfeFullName, sweFullName);

        // Assert
        result.Should().BeLessThan(poorScore);
    }

    public static IEnumerable<object[]> ExactMatchTestData =>
        new List<object[]>
        {
            new object[] { "Candace Ann Bosco", "Candace Ann Bosco" },
            new object[] { "Sam Mac-Donald", "Sam Mac-Donald" },
        };

    public static IEnumerable<object[]> VeryGoodMatchTestData =>
        new List<object[]>
        {
            new object[] { "Ruma Begum", "Rumana Begum" },
            new object[] { "Susan Ann Bennett", "Susan Jane Bennett" },
            new object[] { "Gemma Elizabeth Downes", "Gemma Elizabeth Downesv" },
            new object[] { "Kelly-Marie Jackson", "Kelly Marie Jackson" },
            new object[] { "Donna Louise Archibald", "Clare Louise Archibald" },
            new object[] { "Karen Johnson", "Karen Ann Johnson" }
        };

    public static IEnumerable<object[]> GoodTestData =>
        new List<object[]>
        {
            new object[] { "Jayden Paulo Koch", "Hayden Paul Johnson" },
            new object[] { "Samantha Edwards Jackson", "Samantha Jedwards" },
        };

    public static IEnumerable<object[]> AverageTestData =>
        new List<object[]>
        {
            new object[] { "Mark Anderson", "Mike Paul Hanson" },
            new object[] { "John Paul Smith", "Mike Pauline Hanson" },
        };

    public static IEnumerable<object[]> PoorTestData =>
        new List<object[]> { new object[] { "Judy", "Mark Smith" }, new object[] { "", "" } };

#pragma warning disable xUnit2000
    /// <summary>
    /// https://github.com/apache/lucenenet/blob/38a7b532f30514a351cf436581d8d7fad856a880/src/Lucene.Net.Suggest/Spell/LevensteinDistance.cs
    /// </summary>
    [Fact]
    public void TestGetDistance()
    {
        var d = _sut.MatchConfidence("al", "al");
        Assert.Equal(d, 1.0, 0.001);

        d = _sut.MatchConfidence("martha", "marhta");
        Assert.Equal(d, 0.6666, 0.001);

        d = _sut.MatchConfidence("jones", "johnson");
        Assert.Equal(d, 0.4285, 0.001);

        d = _sut.MatchConfidence("abcvwxyz", "cabvwxyz");
        Assert.Equal(d, 0.75, 0.001);

        d = _sut.MatchConfidence("dwayne", "duane");
        Assert.Equal(d, 0.666, 0.001);

        d = _sut.MatchConfidence("dixon", "dicksonx");
        Assert.Equal(d, 0.5, 0.001);

        d = _sut.MatchConfidence("six", "ten");
        Assert.Equal(d, 0.0, 0.001);

        var d2 = _sut.MatchConfidence("zac ephron", "kai ephron");
        var d1 = _sut.MatchConfidence("zac ephron", "zac efron");
        Assert.Equal(d1, d2, 0.001);

        d1 = _sut.MatchConfidence("brittney spears", "britney spears");
        d2 = _sut.MatchConfidence("brittney spears", "brittney startzman");
        Assert.True(d1 > d2);
    }

    [Fact]
    public void TestEmpty()
    {
        var d = _sut.MatchConfidence("", "al");
        Assert.Equal(d, 0.0, 0.001);
    }
#pragma warning restore xUnit2000
}
