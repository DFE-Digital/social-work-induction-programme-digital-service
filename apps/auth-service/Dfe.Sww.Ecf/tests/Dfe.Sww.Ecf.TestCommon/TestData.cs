using System.Diagnostics;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;

namespace Dfe.Sww.Ecf.TestCommon;

public partial class TestData
{
    private static readonly object _gate = new();
    private static readonly HashSet<string> _emails = [];
    private static readonly HashSet<string> _mobileNumbers = [];

    private readonly Func<Task<string>> _generateTrn;

    public TestData(
        IDbContextFactory<EcfDbContext> dbContextFactory,
        IClock clock,
        FakeTrnGenerator trnGenerator)
        : this(
              dbContextFactory,
              clock,
              generateTrn: () => Task.FromResult(trnGenerator.GenerateTrn()))
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
    {
        0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x01, 0x01, 0x00, 0x48, 0x00, 0x48, 0x00, 0x00,
        0xFF, 0xDB, 0x00, 0x43, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xC2, 0x00, 0x0B, 0x08, 0x00, 0x01, 0x00, 0x01, 0x01, 0x01,
        0x11, 0x00, 0xFF, 0xC4, 0x00, 0x14, 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0xFF, 0xDA, 0x00, 0x08, 0x01, 0x01, 0x00, 0x01, 0x3F, 0x10
    };

    public IClock Clock { get; }

    public IDbContextFactory<EcfDbContext> DbContextFactory { get; }


    public static TestData CreateWithCustomTrnGeneration(
        IDbContextFactory<EcfDbContext> dbContextFactory,
        IClock clock,
        Func<Task<string>> generateTrn)
    {
        return new TestData(dbContextFactory, clock, generateTrn);
    }

    public static async Task<string> GetBase64EncodedFileContent(Stream file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var buffer = ms.ToArray();
        return Convert.ToBase64String(buffer);
    }

    public string GenerateApplicationUserName() => Faker.Company.Name();

    public string GenerateChangedApplicationUserName(string currentName)
    {
        string newName;

        do
        {
            newName = GenerateApplicationUserName();
        }
        while (newName == currentName);

        return newName;
    }

    public DateOnly GenerateDateOfBirth() => DateOnly.FromDateTime(Faker.Identification.DateOfBirth());

    public DateOnly GenerateChangedDateOfBirth(DateOnly currentDateOfBirth)
    {
        DateOnly newDateOfBirth;

        do
        {
            newDateOfBirth = GenerateDateOfBirth();
        }
        while (newDateOfBirth == currentDateOfBirth);

        return newDateOfBirth;
    }

    public string GenerateFirstName() => Faker.Name.First();

    public string GenerateChangedFirstName(string currentFirstName)
    {
        string newFirstName;

        do
        {
            newFirstName = GenerateLastName();
        }
        while (newFirstName == currentFirstName);

        return newFirstName;
    }

    public string GenerateMiddleName() => Faker.Name.Middle();

    public string GenerateChangedMiddleName(string currentMiddleName)
    {
        string newMiddleName;

        do
        {
            newMiddleName = GenerateMiddleName();
        }
        while (newMiddleName == currentMiddleName);

        return newMiddleName;
    }

    public string GenerateLastName() => Faker.Name.Last();

    public string GenerateChangedLastName(string currentLastName)
    {
        string newLastName;

        do
        {
            newLastName = GenerateLastName();
        }
        while (newLastName == currentLastName);

        return newLastName;
    }

    public string GenerateName() => Faker.Name.FullName();

    public string GenerateChangedName(string currentName)
    {
        string newName;

        do
        {
            newName = GenerateName();
        }
        while (newName == currentName);

        return newName;
    }

    public string GenerateChangedNationalInsuranceNumber(string currentNationalInsuranceNumber)
    {
        string newNationalInsuranceNumber;

        do
        {
            newNationalInsuranceNumber = GenerateNationalInsuranceNumber();
        }
        while (newNationalInsuranceNumber == currentNationalInsuranceNumber);

        return newNationalInsuranceNumber;
    }

    public string GenerateUniqueEmail()
    {
        string email;

        lock (_gate)
        {
            do
            {
                email = Faker.Internet.Email();
            }
            while (!_emails.Add(email));
        }

        return email;
    }

    public string GenerateUniqueMobileNumber()
    {
        string mobileNumber;

        lock (_gate)
        {
            do
            {
                mobileNumber = Faker.Phone.Number();
            }
            while (!_mobileNumbers.Add(mobileNumber));
        }

        return mobileNumber;
    }

    public Task<string> GenerateTrn() => _generateTrn();

    public DateOnly GenerateDate(DateOnly min, DateOnly? max = null)
    {
        if (max is not null && max <= min)
        {
            throw new ArgumentOutOfRangeException(nameof(max), "max must be after min.");
        }

        max ??= min.AddYears(1);

        var daysDiff = (int)(max.Value.ToDateTime(TimeOnly.MinValue) - min.ToDateTime(TimeOnly.MinValue)).TotalDays;
        Debug.Assert(daysDiff > 0);
        return min.AddDays(Random.Shared.Next(minValue: 1, maxValue: daysDiff + 1));
    }

    public DateOnly GenerateChangedDate(DateOnly currentDate, DateOnly min, DateOnly? max = null)
    {
        DateOnly newDate;

        do
        {
            newDate = GenerateDate(min, max);
        }
        while (newDate == currentDate);

        return newDate;
    }

    public string GenerateNationalInsuranceNumber() => Faker.Identification.UkNationalInsuranceNumber();

    protected async Task<T> WithDbContext<T>(Func<EcfDbContext, Task<T>> action)
    {
        using var dbContext = await DbContextFactory.CreateDbContextAsync();
        return await action(dbContext);
    }

    protected async Task WithDbContext(Func<EcfDbContext, Task> action)
    {
        using var dbContext = await DbContextFactory.CreateDbContextAsync();
        await action(dbContext);
    }
}
