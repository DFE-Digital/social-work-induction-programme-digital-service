using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Services.PersonMatching;

namespace Dfe.Sww.Ecf.Core.Tests.Services.PersonMatching;

[Collection(nameof(DisableParallelization))]
public class PersonMatchingServiceTests : IAsyncLifetime
{
    public PersonMatchingServiceTests(
        DbFixture dbFixture,
        FakeTrnGenerator trnGenerator)
    {
        DbFixture = dbFixture;
        Clock = new();

        var dbContextFactory = dbFixture.GetDbContextFactory();
        TestData = new TestData(
            dbContextFactory,
            Clock,
            trnGenerator);
    }

    private DbFixture DbFixture { get; }

    private TestData TestData { get; }

    private TestableClock Clock { get; }

    public Task InitializeAsync() => DbFixture.DbHelper.ClearData();

    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [MemberData(nameof(MatchData))]
    public Task Match_ReturnsExpectedResult(
            NameArgumentOption nameOption,
            DateOfBirthArgumentOption dateOfBirthOption,
            NationalInsuranceNumberArgumentOption nationalInsuranceNumberOption,
            TrnArgumentOption trnOption,
            bool expectMatch,
            IEnumerable<OneLoginUserMatchedAttribute>? expectedMatchedAttributes) =>
        DbFixture.WithDbContext(async dbContext =>
        {
            // Arrange
            var firstName = TestData.GenerateFirstName();

            var alias = nameOption == NameArgumentOption.MatchesAlias ? TestData.GenerateFirstName() : null;
            if (alias is not null)
            {
                dbContext.NameSynonyms.Add(new NameSynonyms()
                {
                    Name = firstName,
                    Synonyms = [alias],
                });
                await dbContext.SaveChangesAsync();
            }

            var person = await TestData.CreatePerson(b => b.WithTrn().WithNationalInsuranceNumber().WithFirstName(firstName));

            string[][] names = nameOption switch
            {
                NameArgumentOption.NoFullName => [[person.FirstName]],
                NameArgumentOption.MatchesPersonName => [[person.FirstName, person.LastName]],
                NameArgumentOption.MultipleSpecifiedAndOneMatchesPersonName => [[person.FirstName, person.LastName], [TestData.GenerateChangedFirstName(person.FirstName), person.LastName]],
                NameArgumentOption.MatchesAlias => [[alias!, person.LastName]],
                NameArgumentOption.SpecifiedButDifferentFirstName => [[TestData.GenerateChangedFirstName(person.FirstName), person.LastName]],
                NameArgumentOption.SpecifiedButDifferentLastName => [[person.FirstName, TestData.GenerateChangedLastName(person.LastName)]],
                _ => [],
            };

            DateOnly[] datesOfBirth = dateOfBirthOption switch
            {
                DateOfBirthArgumentOption.MatchesPersonDateOfBirth => [person.DateOfBirth],
                DateOfBirthArgumentOption.MultipleSpecifiedAndOneMatchesPersonDateOfBirth => [person.DateOfBirth, TestData.GenerateChangedDateOfBirth(person.DateOfBirth)],
                DateOfBirthArgumentOption.SpecifiedButDifferent => [TestData.GenerateChangedDateOfBirth(person.DateOfBirth)],
                _ => []
            };

            var nationalInsuranceNumber = nationalInsuranceNumberOption switch
            {
                NationalInsuranceNumberArgumentOption.SpecifiedAndMatchesPersonNino => person.NationalInsuranceNumber!,
                NationalInsuranceNumberArgumentOption.SpecifiedButDifferent => TestData.GenerateChangedNationalInsuranceNumber(person.NationalInsuranceNumber!),
                _ => null
            };

            var trn = trnOption switch
            {
                TrnArgumentOption.SpecifiedAndMatches => person.Trn!,
                TrnArgumentOption.SpecifiedButDifferent => await TestData.GenerateTrn(),
                _ => null
            };

            var service = new PersonMatchingService(dbContext);

            // Act
            var result = await service.Match(new(names, datesOfBirth, nationalInsuranceNumber, trn));

            // Assert
            if (expectMatch)
            {
                Assert.NotNull(result);
                Assert.Equal(person.PersonId, result.PersonId);
                Assert.Equal(person.Trn, result.Trn);
                Assert.Equal(expectedMatchedAttributes?.Order(), result.MatchedAttributes.Select(kvp => kvp.Key).Distinct().Order());
            }
            else
            {
                Assert.Null(result);
            }
        });

    [Fact]
    public Task Match_WithMultipleMatchingResults_ReturnsNull() =>
        DbFixture.WithDbContext(async dbContext =>
        {
            // Arrange
            var firstName = TestData.GenerateFirstName();
            var lastName = TestData.GenerateLastName();
            var dateOfBirth = TestData.GenerateDateOfBirth();

            var person1 = await TestData.CreatePerson(b => b.WithTrn().WithNationalInsuranceNumber().WithFirstName(firstName).WithLastName(lastName).WithDateOfBirth(dateOfBirth));
            var person2 = await TestData.CreatePerson(b => b.WithTrn().WithNationalInsuranceNumber().WithFirstName(firstName).WithLastName(lastName).WithDateOfBirth(dateOfBirth));

            string[][] names = [[firstName, lastName]];
            DateOnly[] datesOfBirth = [dateOfBirth];
            var nationalInsuranceNumber = person1.NationalInsuranceNumber!;
            var trn = person2.Trn!;

            var service = new PersonMatchingService(dbContext);

            // Act
            var result = await service.Match(new(names, datesOfBirth, nationalInsuranceNumber, trn));

            // Assert
            Assert.Null(result);
        });

    [Fact]
    public Task Match_WithMultipleMatchingNames_ReturnsResult() =>
        DbFixture.WithDbContext(async dbContext =>
        {
            // Arrange
            var firstName = Guid.NewGuid().ToString();  // Deliberately weird first name to avoid unique constraint violations in NameSynonyms below

            var alias = TestData.GenerateFirstName();
            dbContext.NameSynonyms.Add(new NameSynonyms()
            {
                Name = firstName,
                Synonyms = [alias],
            });
            await dbContext.SaveChangesAsync();

            var person = await TestData.CreatePerson(b => b.WithTrn().WithNationalInsuranceNumber().WithFirstName(firstName));

            string[][] names = [[person.FirstName, person.LastName], [alias, person.LastName]];
            DateOnly[] datesOfBirth = [person.DateOfBirth];
            var nationalInsuranceNumber = person.NationalInsuranceNumber!;
            var trn = person.Trn!;

            var service = new PersonMatchingService(dbContext);

            // Act
            var result = await service.Match(new(names, datesOfBirth, nationalInsuranceNumber, trn));

            // Assert
            Assert.NotNull(result);
        });

    [Fact]
    public Task GetSuggestedMatches_ReturnsExpectedResults() =>
        DbFixture.WithDbContext(async dbContext =>
        {
            // Arrange
            var firstName = TestData.GenerateFirstName();
            var lastName = TestData.GenerateLastName();
            var dateOfBirth = TestData.GenerateDateOfBirth();
            var nationalInsuranceNumber = TestData.GenerateNationalInsuranceNumber();

            // Person who matches on last name & DOB
            var person1 = await TestData.CreatePerson(b => b.WithLastName(lastName).WithDateOfBirth(dateOfBirth));

            // Person who matches on TRN
            var person2 = await TestData.CreatePerson(b => b.WithTrn());
            var trn = person2.Trn!;

            // Person who matches on last name, DOB & TRN
            var person3 = await TestData.CreatePerson(b => b.WithTrn().WithLastName(lastName).WithDateOfBirth(dateOfBirth));
            var trnTokenHintTrn = person3.Trn!;

            string[][] names = [[firstName, lastName]];
            DateOnly[] datesOfBirth = [dateOfBirth];

            var service = new PersonMatchingService(dbContext);

            // Act
            var result = await service.GetSuggestedMatches(new(names, datesOfBirth, nationalInsuranceNumber, trn, trnTokenHintTrn));

            // Assert
            Assert.Collection(
                result,
                r => Assert.Equal(person3.PersonId, r.PersonId),
                r => Assert.Equal(person2.PersonId, r.PersonId),
                r => Assert.Equal(person1.PersonId, r.PersonId));
        });

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public Task GetMatchedAttributes_ReturnsExpectedResults(bool usePersonNino) =>
        DbFixture.WithDbContext(async dbContext =>
        {
            // Arrange
            var firstName = TestData.GenerateFirstName();
            var lastName = TestData.GenerateLastName();
            var dateOfBirth = TestData.GenerateDateOfBirth();
            var nationalInsuranceNumber = TestData.GenerateNationalInsuranceNumber();
            var alternativeNationalInsuranceNumber = TestData.GenerateChangedNationalInsuranceNumber(nationalInsuranceNumber);

            var person = await TestData.CreatePerson(b => b.WithFirstName(firstName).WithLastName(lastName).WithDateOfBirth(dateOfBirth).WithNationalInsuranceNumber(usePersonNino ? nationalInsuranceNumber : alternativeNationalInsuranceNumber));
            //var establishment = await TestData.CreateOrganisation(organisationName: "Test LA");
            //var personEmployment = await TestData.CreatePersonOrganisation(person, establishment, new DateOnly(2023, 08, 03));
            string[][] names = [[firstName, lastName]];
            DateOnly[] datesOfBirth = [dateOfBirth];

            var service = new PersonMatchingService(dbContext);

            // Act
            var result = await service.GetMatchedAttributes(new(names, datesOfBirth, nationalInsuranceNumber, person.Trn!, TrnTokenTrnHint: null), person.PersonId);

            // Assert
            Assert.Collection(
                result,
                m => AssertAttributeMatch(OneLoginUserMatchedAttribute.FullName, $"{firstName} {lastName}", m),
                m => AssertAttributeMatch(OneLoginUserMatchedAttribute.LastName, lastName, m),
                m => AssertAttributeMatch(OneLoginUserMatchedAttribute.DateOfBirth, dateOfBirth.ToString("yyyy-MM-dd"), m),
                m => AssertAttributeMatch(OneLoginUserMatchedAttribute.NationalInsuranceNumber, nationalInsuranceNumber, m),
                m => AssertAttributeMatch(OneLoginUserMatchedAttribute.Trn, person.Trn!, m),
                m => AssertAttributeMatch(OneLoginUserMatchedAttribute.FirstName, firstName, m));

            static void AssertAttributeMatch(OneLoginUserMatchedAttribute expectedAttribute, string expectedValue, KeyValuePair<OneLoginUserMatchedAttribute, string> actual)
            {
                Assert.Equal(expectedAttribute, actual.Key);
                Assert.Equal(expectedValue, actual.Value);
            }
        });

    private static readonly OneLoginUserMatchedAttribute[] _matchNameDobNinoAndTrnAttributes =
    [
        OneLoginUserMatchedAttribute.FullName,
        OneLoginUserMatchedAttribute.DateOfBirth,
        OneLoginUserMatchedAttribute.NationalInsuranceNumber,
        OneLoginUserMatchedAttribute.Trn
    ];

    private static readonly OneLoginUserMatchedAttribute[] _matchNameDobAndNinoAttributes =
    [
        OneLoginUserMatchedAttribute.FullName,
        OneLoginUserMatchedAttribute.DateOfBirth,
        OneLoginUserMatchedAttribute.NationalInsuranceNumber
    ];

    private static readonly OneLoginUserMatchedAttribute[] _matchNameDobAndTrnAttributes =
    [
        OneLoginUserMatchedAttribute.FullName,
        OneLoginUserMatchedAttribute.DateOfBirth,
        OneLoginUserMatchedAttribute.Trn
    ];

    public static TheoryData<NameArgumentOption, DateOfBirthArgumentOption, NationalInsuranceNumberArgumentOption, TrnArgumentOption, bool, IEnumerable<OneLoginUserMatchedAttribute>?> MatchData { get; } = new()
    {
        // *** Match cases ***

        // Single name, single DOB, person NINO and TRN all match
        {
            NameArgumentOption.MatchesPersonName,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.SpecifiedAndMatchesPersonNino,
            TrnArgumentOption.SpecifiedAndMatches,
            /*expectMatch: */ true,
            _matchNameDobNinoAndTrnAttributes
        },

        // Single name, single DOB, person NINO match but no TRN
        {
            NameArgumentOption.MatchesPersonName,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.SpecifiedAndMatchesPersonNino,
            TrnArgumentOption.NotSpecified,
            /*expectMatch: */ true,
            _matchNameDobAndNinoAttributes
        },

        // Single name, single DOB, person NINO match but TRN doesn't match
        {
            NameArgumentOption.MatchesPersonName,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.SpecifiedAndMatchesPersonNino,
            TrnArgumentOption.SpecifiedButDifferent,
            /*expectMatch: */ true,
            _matchNameDobAndNinoAttributes
        },

        // Single name, single DOB, TRN match but no NINO
        {
            NameArgumentOption.MatchesPersonName,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.NotSpecified,
            TrnArgumentOption.SpecifiedAndMatches,
            /*expectMatch: */ true,
            _matchNameDobAndTrnAttributes
        },

        // Single name, single DOB, TRN match but person NINO doesn't match
        {
            NameArgumentOption.MatchesPersonName,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.SpecifiedButDifferent,
            TrnArgumentOption.SpecifiedAndMatches,
            /*expectMatch: */ true,
            _matchNameDobAndTrnAttributes
        },

        // Single name with alias, single DOB, person NINO and TRN all match
        {
            NameArgumentOption.MatchesAlias,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.SpecifiedAndMatchesPersonNino,
            TrnArgumentOption.SpecifiedAndMatches,
            /*expectMatch: */ true,
            _matchNameDobNinoAndTrnAttributes
        },

        // Single name with alias, single DOB, person NINO match but no TRN
        {
            NameArgumentOption.MatchesAlias,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.SpecifiedAndMatchesPersonNino,
            TrnArgumentOption.NotSpecified,
            /*expectMatch: */ true,
            _matchNameDobAndNinoAttributes
        },

        // Single name with alias, single DOB, person NINO match but TRN doesn't match
        {
            NameArgumentOption.MatchesAlias,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.SpecifiedAndMatchesPersonNino,
            TrnArgumentOption.SpecifiedButDifferent,
            /*expectMatch: */ true,
            _matchNameDobAndNinoAttributes
        },

        // Single name with alias, single DOB, TRN match but no NINO
        {
            NameArgumentOption.MatchesAlias,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.NotSpecified,
            TrnArgumentOption.SpecifiedAndMatches,
            /*expectMatch: */ true,
            _matchNameDobAndTrnAttributes
        },

        // Single name with alias, single DOB, TRN match but person NINO doesn't match
        {
            NameArgumentOption.MatchesAlias,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.SpecifiedButDifferent,
            TrnArgumentOption.SpecifiedAndMatches,
            /*expectMatch: */ true,
            _matchNameDobAndTrnAttributes
        },

        // Multiple names with one match, single DOB, person NINO and TRN all match
        {
            NameArgumentOption.MultipleSpecifiedAndOneMatchesPersonName,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.SpecifiedAndMatchesPersonNino,
            TrnArgumentOption.SpecifiedAndMatches,
            /*expectMatch: */ true,
            _matchNameDobNinoAndTrnAttributes
        },

        // Multiple names with one match, single DOB, person NINO match but no TRN
        {
            NameArgumentOption.MultipleSpecifiedAndOneMatchesPersonName,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.SpecifiedAndMatchesPersonNino,
            TrnArgumentOption.NotSpecified,
            /*expectMatch: */ true,
            _matchNameDobAndNinoAttributes
        },

        // Multiple names with one match, single DOB, person NINO match but TRN doesn't match
        {
            NameArgumentOption.MultipleSpecifiedAndOneMatchesPersonName,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.SpecifiedAndMatchesPersonNino,
            TrnArgumentOption.SpecifiedButDifferent,
            /*expectMatch: */ true,
            _matchNameDobAndNinoAttributes
        },

        // Multiple names with one match, single DOB, TRN match but no person NINO
        {
            NameArgumentOption.MultipleSpecifiedAndOneMatchesPersonName,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.NotSpecified,
            TrnArgumentOption.SpecifiedAndMatches,
            /*expectMatch: */ true,
            _matchNameDobAndTrnAttributes
        },

        // Multiple names with one match, single DOB, TRN match but person NINO doesn't match
        {
            NameArgumentOption.MultipleSpecifiedAndOneMatchesPersonName,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.SpecifiedButDifferent,
            TrnArgumentOption.SpecifiedAndMatches,
            /*expectMatch: */ true,
            _matchNameDobAndTrnAttributes
        },

        // *** No match cases ***

        // Missing names
        {
            NameArgumentOption.NotSpecifed,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.SpecifiedAndMatchesPersonNino,
            TrnArgumentOption.SpecifiedAndMatches,
            /*expectMatch: */ false,
            null
        },

        // Missing full name
        {
            NameArgumentOption.NoFullName,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.SpecifiedAndMatchesPersonNino,
            TrnArgumentOption.SpecifiedAndMatches,
            /*expectMatch: */ false,
            null
        },

        // Missing dates of birth
        {
            NameArgumentOption.MatchesPersonName,
            DateOfBirthArgumentOption.NotSpecifed,
            NationalInsuranceNumberArgumentOption.SpecifiedAndMatchesPersonNino,
            TrnArgumentOption.SpecifiedAndMatches,
            /*expectMatch: */ false,
            null
        },

        // Missing TRN and NINO
        {
            NameArgumentOption.MatchesPersonName,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.NotSpecified,
            TrnArgumentOption.NotSpecified,
            /*expectMatch: */ false,
            null
        },

        // First name doesn't match
        {
            NameArgumentOption.SpecifiedButDifferentFirstName,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.NotSpecified,
            TrnArgumentOption.NotSpecified,
            /*expectMatch: */ false,
            null
        },

        // Last name doesn't match
        {
            NameArgumentOption.SpecifiedButDifferentLastName,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.NotSpecified,
            TrnArgumentOption.NotSpecified,
            /*expectMatch: */ false,
            null
        },

        // DOB doesn't match
        {
            NameArgumentOption.SpecifiedButDifferentLastName,
            DateOfBirthArgumentOption.SpecifiedButDifferent,
            NationalInsuranceNumberArgumentOption.NotSpecified,
            TrnArgumentOption.NotSpecified,
            /*expectMatch: */ false,
            null
        },

        // Neither NINO nor TRN match
        {
            NameArgumentOption.SpecifiedButDifferentLastName,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.SpecifiedButDifferent,
            TrnArgumentOption.SpecifiedButDifferent,
            /*expectMatch: */ false,
            null
        },

        // NINO doesn't match, TRN not specified
        {
            NameArgumentOption.MatchesPersonName,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.SpecifiedButDifferent,
            TrnArgumentOption.NotSpecified,
            /*expectMatch: */ false,
            null
        },

        // TRN doesn't match, NINO not specified
        {
            NameArgumentOption.MatchesPersonName,
            DateOfBirthArgumentOption.MatchesPersonDateOfBirth,
            NationalInsuranceNumberArgumentOption.NotSpecified,
            TrnArgumentOption.SpecifiedButDifferent,
            /*expectMatch: */ false,
            null
        },
    };

    public enum NameArgumentOption
    {
        NotSpecifed,
        NoFullName,
        MatchesPersonName,
        MultipleSpecifiedAndOneMatchesPersonName,
        MatchesAlias,
        SpecifiedButDifferentFirstName,
        SpecifiedButDifferentLastName
    }

    public enum DateOfBirthArgumentOption
    {
        NotSpecifed,
        MatchesPersonDateOfBirth,
        MultipleSpecifiedAndOneMatchesPersonDateOfBirth,
        SpecifiedButDifferent
    }

    public enum NationalInsuranceNumberArgumentOption
    {
        NotSpecified,
        SpecifiedAndMatchesPersonNino,
        SpecifiedButDifferent
    }

    public enum TrnArgumentOption
    {
        NotSpecified,
        SpecifiedAndMatches,
        SpecifiedButDifferent
    }
}
