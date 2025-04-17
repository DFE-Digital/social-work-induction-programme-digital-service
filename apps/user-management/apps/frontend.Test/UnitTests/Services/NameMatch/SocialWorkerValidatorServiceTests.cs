using Dfe.Sww.Ecf.Frontend.Models.NameMatch;
using Dfe.Sww.Ecf.Frontend.Services.NameMatch;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.NameMatch;

public class SocialWorkerValidatorServiceTests
{
    private readonly SocialWorkerValidatorService _sut;

    public SocialWorkerValidatorServiceTests(ITestOutputHelper testOutputHelper)
    {
        _sut = new();
    }

    [Theory]
    [MemberData(nameof(PassingTestData))]
    public void ConvertToResult_WhenPassedNames_ShouldReturnCorrectResponse(
        string firstName,
        string lastName,
        string fullName,
        MatchResult expectedResult
    )
    {
        // Act
        var result = _sut.ConvertToResult(firstName, lastName, fullName);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData("", "", "John Smith")]
    public void ConvertToResult_WhenPassedInvalidNames_ShouldThrowArgumentNullException(
        string? firstName,
        string? lastName,
        string? fullName
    )
    {
        // Arrange
        var expectedException = typeof(ArgumentNullException);

        // Act
        var actualException = Assert.Throws<ArgumentNullException>(
            () => _sut.ConvertToResult(firstName, lastName, fullName)
        );

        // Assert
        actualException.Should().BeOfType(expectedException);
    }

    [Fact]
    public void ConvertToResult_WhenPassedReallyLongNames_ShouldReturnConsistentResults()
    {
        // Arrange
        var firstName = "Ana";
        var lastName = "Costa";
        var sweName = "Ana Salome Do Amaral Pinto Soares Da Costa";

        // Return a list of 100 randomised middle names. Random order & random number of names removed
        var randomMiddleNamesList = GetRandomMiddleNames(sweName, 100);

        // Input the names into the algorithm in 4 different formats
        // 1. First Name - "{A} {B}" | Last Name - "{C}"
        // 2. First Name - "{A}" | Last Name - "{B} {C}"
        // 3. First Name - "{A} {B} {C}" | Last Name - ""
        // 4. First Name - "" | Last Name - "{A} {B} {C}"
        foreach (var randomValue in randomMiddleNamesList)
        {
            var scores = new HashSet<double>();

            var resultA = _sut.Normalise($"{firstName} {randomValue}", lastName, sweName);
            scores.Add(resultA);

            var resultB = _sut.Normalise(firstName, $"{randomValue} {lastName}", sweName);
            scores.Add(resultB);

            var resultC = _sut.Normalise(
                $"{firstName} {randomValue} {lastName}",
                string.Empty,
                sweName
            );
            scores.Add(resultC);

            var resultD = _sut.Normalise(
                string.Empty,
                $"{firstName} {randomValue} {lastName}",
                sweName
            );
            scores.Add(resultD);

            // Assert the scores
            if (scores.Count != 1)
            {
                Assert.Fail(
                    $"Test failed using random value \"{randomValue}\", first name \"{firstName}\", last name \"{lastName}\""
                );
            }
        }
    }

    public static IEnumerable<object[]> PassingTestData =>
        new List<object[]>
        {
            // Exact
            new object[] { "John", "Smith", "John Smith", MatchResult.Exact },
            // Very Good
            new object[] { "Kelly-Marie", "Jackson", "Kelly Marie Jackson", MatchResult.VeryGood },
            new object[] { "Ruma", "Begum", "Rumana Begum", MatchResult.VeryGood },
            new object[] { "Karen", "Johnson", "Karen Ann Johnson", MatchResult.Good },
            new object[]
            {
                "Gemma",
                "Elizabeth Downes",
                "Gemma Elizabeth Downesv",
                MatchResult.VeryGood
            },
            // Good
            new object[] { "Susan Ann", "Bennett", "Susan Jane Bennett", MatchResult.Good },
            new object[] { "John", "Smith", "Jake Smithe", MatchResult.Good },
            // Average
            new object[] { "Samantha", "", "Dan", MatchResult.Average },
            new object[]
            {
                "Samantha Edwards",
                "Jackson",
                "Samantha Danielle Edwards",
                MatchResult.Average
            },
            // Poor
            new object[] { "Manuel", "Dixon", "Samuel", MatchResult.Poor },
            new object[] { "Mike", "", "Samuel", MatchResult.Poor },
            new object[] { "Simon", "James", "Mixie", MatchResult.Poor },
            // Fail
            new object[] { "Simon", "", "Jane", MatchResult.Fail },
            new object[] { "Allan", "", "Tim", MatchResult.Fail },
            new object[] { "Allan", "Smith", "", MatchResult.Fail },
        };

    private List<string> GetRandomMiddleNames(string fullName, int randomValuesCount)
    {
        var nameSplit = fullName.Split(" ");
        var random = new Random(67);

        var response = new List<string>();

        var middleNames = nameSplit.Skip(1).SkipLast(1).ToArray();
        var n = middleNames.Length;

        for (var i = 0; i < randomValuesCount; i++)
        {
            var randomInt = random.Next(1, n);

            random.Shuffle(middleNames);

            var randomMiddleNames = middleNames[..randomInt];
            var joinedMiddleName = string.Join(" ", randomMiddleNames);

            response.Add(joinedMiddleName);
        }

        return response;
    }
}
