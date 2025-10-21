using System.Diagnostics;
using Bogus;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.TestCommon.Extensions;

namespace Dfe.Sww.Ecf.TestCommon;

public partial class TestData
{
    private static readonly Faker _faker = new("en_GB");

    private readonly Func<Task<string>> _generateTrn;

    public TestData(
        IDbContextFactory<EcfDbContext> dbContextFactory,
        IClock clock,
        FakeTrnGenerator trnGenerator)
        : this(
            dbContextFactory,
            clock,
            () => Task.FromResult(trnGenerator.GenerateTrn()))
    {
    }

    private TestData(
        IDbContextFactory<EcfDbContext> dbContextFactory,
        IClock clock,
        Func<Task<string>> generateTrn)
    {
        DbContextFactory = dbContextFactory;
        Clock = clock;
        _generateTrn = generateTrn;
    }

    // https://stackoverflow.com/a/30290754
    public static byte[] JpegImage { get; } =
    [
        0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x01, 0x01, 0x00, 0x48, 0x00, 0x48,
        0x00, 0x00,
        0xFF, 0xDB, 0x00, 0x43, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xC2, 0x00, 0x0B, 0x08, 0x00, 0x01, 0x00, 0x01,
        0x01, 0x01,
        0x11, 0x00, 0xFF, 0xC4, 0x00, 0x14, 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0xFF, 0xDA, 0x00, 0x08, 0x01, 0x01, 0x00, 0x01, 0x3F, 0x10
    ];

    private IClock Clock { get; }

    private IDbContextFactory<EcfDbContext> DbContextFactory { get; }


    private static DateOnly GenerateDateOfBirth()
    {
        return DateOnly.FromDateTime(_faker.Person.DateOfBirth);
    }

    private static string GenerateFirstName()
    {
        return _faker.Name.FirstName();
    }

    private static string GenerateMiddleName()
    {
        return _faker.Name.FirstName();
    }

    private static string GenerateLastName()
    {
        return _faker.Name.LastName();
    }

    private static string GenerateEmail()
    {
        return _faker.Internet.Email();
    }

    public Task<string> GenerateTrn()
    {
        return _generateTrn();
    }

    private static DateOnly GenerateDate(DateOnly min, DateOnly? max = null)
    {
        if (max <= min)
        {
            throw new ArgumentOutOfRangeException(nameof(max), "max must be after min.");
        }

        max ??= min.AddYears(1);

        var daysDiff = (int)(max.Value.ToDateTime(TimeOnly.MinValue) - min.ToDateTime(TimeOnly.MinValue)).TotalDays;
        Debug.Assert(daysDiff > 0);
        return min.AddDays(Random.Shared.Next(1, daysDiff + 1));
    }

    public static string GenerateNationalInsuranceNumber()
    {
        return _faker.Person.UkNationalInsuranceNumber();
    }

    private async Task<T> WithDbContext<T>(Func<EcfDbContext, Task<T>> action)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();
        return await action(dbContext);
    }
}
