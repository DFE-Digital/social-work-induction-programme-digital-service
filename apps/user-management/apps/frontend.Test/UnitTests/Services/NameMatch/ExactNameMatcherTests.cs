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
            new object[] { "John Michael Smith", "John Michael Smith" },
            new object[] { "John Smith", "John Smith" },
            new object[] { "Muhammad", "Muhammad" },
            new object[] { "John Smith", "John Smith" },
            new object[] { "Jane Marie Doe", "Jane Marie Doe" },
            new object[] { "J. Smith", "J. Smith" },
            new object[] { "Samantha Doe", "Samantha Doe" },
            new object[] { "Annmarie Johnson", "Annmarie Johnson" },
            new object[] { "Marta Stewart", "Marta Stewart" },
            new object[] { "Mary Jane Smith", "Mary Jane Smith" },
            new object[] { "Christopher Johansen", "Christopher Johansen" },
            new object[] { "Marhta", "Marhta" },
            new object[] { "Dicksonx", "Dicksonx" },
            new object[] { "Ben Carter", "Ben Carter" },
            new object[] { "Bella Thompson", "Bella Thompson" },
            new object[] { "Mikhail Peter", "Mikhail Peter" },
            new object[] { "Michelle Anne Robinson", "Michelle Anne Robinson" },
            new object[] { "Michael Robin Camp", "Michael Robin Camp" },
            new object[] { "Michael Robin Camp", "Michael Robin Camp" },
            new object[] { "Andrew Philip Jones", "Andrew Philip Jones" },
            new object[] { "Paul Johnston", "Paul Johnston" },
            new object[] { "supercalifragilisticexpialidocious", "supercalifragilisticexpialidocious" },
            new object[] { "John Smith", "John Smith" },
            new object[] { "Apple", "Apple" },
            new object[] { "Jonathan", "Jonathan" },
            new object[] { "Michael", "Michael" },
            new object[] { "Ann", "Ann" },
            new object[] { "José", "José" },
            new object[] { "supercalifragilisticexpialidocious", "supercalifragilisticexpialidocious" },
            new object[] { "John", "John" },
            new object[] { "and", "and" },
            new object[] { "Christopher", "Christopher" },
        };

    public static IEnumerable<object[]> UnsuccessfulTestData =>
        new List<object[]>
        {
            new object[] { "John Smith", "Jane Doe" },
            new object[] { "Smith", "John Smith" },
            new object[] { "John", "John Smith" },

            new object[] { "John Michael Smith", "John Smith" },
            new object[] { "Jon Smith", "John Smith" },
            new object[] { "Muhamed", "Muhammad" },
            new object[] { "John Smith", "Jane Smith" },
            new object[] { "Jane Marie Doe", "Jane Doe" },
            new object[] { "J. Smith", "John Smith" },
            new object[] { "Samantha Doe", "Samantha Smith" },
            new object[] { "Annmarie Johnson", "Ann Johnson" },
            new object[] { "Marta Stewart", "Marta Jones" },
            new object[] { "Mary Jane Smith", "Mary Smith" },
            new object[] { "Christopher Johansen", "Chris Johansen" },
            new object[] { "Marhta", "Martha" },
            new object[] { "Dicksonx", "Dixon" },
            new object[] { "Ben Carter", "Benjamin Carter" },
            new object[] { "Bella Thompson", "Isabella Thompson" },
            new object[] { "Mikhail Peter", "Michael Peters" },
            new object[] { "Michelle Anne Robinson", "Michelle Jayne Robinson" },
            new object[] { "Michael Leslie Cornfield", "Michelle Teri Schofield" },
            new object[] { "Michael Robin Harper-Coulson", "Michael Robin Camp" },
            new object[] { "Michael Brown", "Michael Robin Camp" },
            new object[] { "Andrew Philip Cooper", "Andrew Philip Jones" },
            new object[] { "Paul John Anderson", "Paul Johnston" },
            new object[] { "supercalifragilisticexpialidocious123", "supercalifragilisticexpialidocious" },
            new object[] { "John Smith", "Smith John" },
            new object[] { "Apple", "Aplle" },
            new object[] { "Jonathan", "Johnathan" },
            new object[] { "Mike", "Michael" },
            new object[] { "Annie", "Ann" },
            new object[] { "Jose", "José" },
            new object[] { "supercalifragilisticexpialidocious123", "supercalifragilisticexpialidocious" },
            new object[] { "John", "Jon" },
            new object[] { "and", "adn" },
            new object[] { "Christoph", "Christopher" }
        };
}
