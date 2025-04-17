using Dfe.Sww.Ecf.Frontend.Services.NameMatch;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.NameMatch;

public class JaroWinklerDistanceMatcherTests
{
    private readonly JaroWinklerDistanceMatcher _sut;

    public JaroWinklerDistanceMatcherTests()
    {
        _sut = new();
    }

    [Theory, MemberData(nameof(ExactMatchTestData))]
    public void WhenPassedExactMatchingNames_ReturnsExpectedScore(
        string dfeFullname,
        string sweFullName
    )
    {
        // Arrange
        var exactScore = 1;

        //Act
        var result = _sut.MatchConfidence(dfeFullname, sweFullName);

        // Assert
        result.Should().Be(exactScore);
    }

    [Theory, MemberData(nameof(VeryGoodMatchTestData))]
    public void WhenPassedVeryGoodNames_ReturnsExpectedScore(string dfeFullname, string sweFullName)
    {
        // Arrange
        var exactMatchScore = 1;
        var veryGoodScore = 0.8;

        //Act
        var result = _sut.MatchConfidence(dfeFullname, sweFullName);

        // Assert
        result.Should().BeLessThan(exactMatchScore);
        result.Should().BeGreaterThanOrEqualTo(veryGoodScore);
    }

    [Theory, MemberData(nameof(GoodTestData))]
    public void WhenPassedGoodNames_ReturnsExpectedScore(string dfeFullname, string sweFullName)
    {
        // Arrange
        var veryGoodScore = 0.8;
        var goodScore = 0.6;

        //Act
        var result = _sut.MatchConfidence(dfeFullname, sweFullName);

        // Assert
        result.Should().BeLessThan(veryGoodScore);
        result.Should().BeGreaterThanOrEqualTo(goodScore);
    }

    [Theory, MemberData(nameof(AverageTestData))]
    public void WhenPassedAverageNames_ReturnsExpectedScore(string dfeFullname, string sweFullName)
    {
        // Arrange
        var goodScore = 0.4;
        var averageScore = 0.2;

        //Act
        var result = _sut.MatchConfidence(dfeFullname, sweFullName);

        // Assert
        result.Should().BeLessThan(goodScore);
        result.Should().BeGreaterThanOrEqualTo(averageScore);
    }

    [Theory, MemberData(nameof(PoorTestData))]
    public void WhenPassedPoorNames_ReturnsExpectedScore(string dfeFullname, string sweFullName)
    {
        // Arrange
        var averageScore = 0.2;
        var poorScore = 0.0;

        //Act
        var result = _sut.MatchConfidence(dfeFullname, sweFullName);

        // Assert
        result.Should().BeLessThan(averageScore);
        result.Should().BeGreaterThanOrEqualTo(poorScore);
    }

    public static IEnumerable<object[]> ExactMatchTestData =>
        new List<object[]>
        {
            new object[] { "Candace Bosco", "Candace Bosco" },
            new object[] { "Sam Mac-Donald", "Sam Mac-Donald" },
            // Additional exact matches
            new object[] { "John Smith", "John Smith" },
            new object[] { "Jane Doe", "Jane Doe" },
            new object[] { "Alice Johnson", "Alice Johnson" },
            new object[] { "Zachary Taylor", "Zachary Taylor" },
            new object[] { "Amanda Clark", "Amanda Clark" },
        };

    public static IEnumerable<object[]> VeryGoodMatchTestData =>
        new List<object[]>
        {
            new object[] { "Donna Louise Archibald", "Clare Louise Archibald" },
            new object[] { "Ruma Begum", "Rumana Begum" },
            new object[] { "Susan Ann Bennett", "Susan Jane Bennett" },
            new object[] { "Gemma Elizabeth Downes", "Gemma Elizabeth Downesv" },
            new object[] { "Karen Johnson", "Karen Ann Johnson" },
            new object[] { "Kelly-Marie Jackson", "Kelly Marie Jackson" },
            // Additional very good matches
            new object[] { "John Smith", "Jon Smith" },
            new object[] { "Jane Doe", "Janet Doe" },
            new object[] { "Alice Johnson", "Alicia Johnson" },
            new object[] { "Zachary Taylor", "Zach Taylor" },
            new object[] { "Amanda Clark", "Amand Clark" },
            new object[] { "Jane Doe", "Jan Doe" },
            new object[] { "John Smith", "Jon Smith" },  // Common typo
            new object[] { "Martha", "Marhta" },         // Slight variation
            new object[] { "José", "Jose" },             // Accented vs. unaccented character
            new object[] { "Ann", "Annie" },              // Nickname vs. full name
            new object[] { "Christopher", "Christoph" },  // Slightly different name
            new object[] { "Michelle Anne Robinson", "Michelle Jayne Robertson" },  // Different middle names
            new object[] { "Paul Johnston", "Paul John Anderson" },                 // Embedded name
            new object[] { "Michelle Teri Schofield", "Michael Leslie Cornfield" }, // Completely different names
            new object[] { "Michael Robin Camp", "Michael Robin Harper-Coulson" }, // Similar first/middle, different last
            new object[] { "John Smith", "John Smyth" },
            new object[] { "Johnathan", "Jonathan" },     // Variant spelling
            new object[] { "Benjamin Carter", "Ben Carter" },  // Shortened first name
            new object[] { "Amanda Clark", "Amand Clarke" },
            new object[] { "Zachary Taylor", "Zachary Tan" },
            new object[] { "Isabella Thompson", "Bella Thompson" }, // Abbreviated first name
        };

    public static IEnumerable<object[]> GoodTestData =>
        new List<object[]>
        {
            new object[] { "Jayden Smith Koch", "Hayden Paul Johnson" },
            new object[] { "Samantha Edwards Jackson", "Samantha Danielle" },
            // Additional good matches
            new object[] { "Zachary Taylor", "Zak Taylor" },
            new object[] { "Alice Johnson", "Bob Johnson" },
            new object[] { "Michael", "Mike" },           // Nickname vs. full name
        };

    public static IEnumerable<object[]> AverageTestData =>
        new List<object[]>
        {
            new object[] { "John Smith", "Paul Ben" },
            new object[] { "Ruma Begum", "John Mall" },
            // Additional average matches
            new object[] { "Alice Johnson", "Bob Marley" },
        };

    public static IEnumerable<object[]> PoorTestData =>
        new List<object[]>
        {
            new object[] { "Gina Park", "Tom" },
            new object[] { "Judy", "Mark Smith" },
            new object[] { "", "" },
        };

    /// <summary>
    /// https://github.com/apache/lucenenet/blob/38a7b532f30514a351cf436581d8d7fad856a880/src/Lucene.Net.Tests.Suggest/Spell/TestJaroWinklerDistance.cs
    /// </summary>
    [Fact]
    public void TestJaroWinklerCompare()
    {
        var d = _sut.MatchConfidence("al", "al");
        d.Should().Be(1.0d);

        d = _sut.MatchConfidence("martha", "marhta");
        d.Should().BeGreaterThan(0.961);
        d.Should().BeLessThan(0.962);

        d = _sut.MatchConfidence("jones", "johnson");
        d.Should().BeGreaterThan(0.832);
        d.Should().BeLessThan(0.833);

        d = _sut.MatchConfidence("abcvwxyz", "cabvwxyz");
        d.Should().BeGreaterThan(0.958);
        d.Should().BeLessThan(0.959);

        d = _sut.MatchConfidence("dwayne", "duane");
        d.Should().BeGreaterThan(0.84);
        d.Should().BeLessThan(0.841);

        d = _sut.MatchConfidence("dixon", "dicksonx");
        d.Should().BeGreaterThan(0.813);
        d.Should().BeLessThan(0.814);

        d = _sut.MatchConfidence("fvie", "ten");
        d.Should().Be(0d);
        d.Should().BeLessThan(0.833);

        var d1 = _sut.MatchConfidence("zac ephron", "zac efron");
        var d2 = _sut.MatchConfidence("zac ephron", "kai ephron");
        d1.Should().BeGreaterThan(d2);

        d1 = _sut.MatchConfidence("brittney spears", "britney spears");
        d2 = _sut.MatchConfidence("brittney spears", "brittney startzman");
        d1.Should().BeGreaterThan(d2);
    }
}
