using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

namespace Dfe.Sww.Ecf.TestCommon;

public partial class TestData
{
    public Task<CreatePersonResult> CreatePerson(Action<CreatePersonBuilder>? configure = null)
    {
        return WithDbContext(async dbContext =>
        {
            var builder = new CreatePersonBuilder();
            configure?.Invoke(builder);
            var createPersonResult = await builder.Execute(this);

            dbContext.Persons.Add(new Person()
            {
                PersonId = createPersonResult.PersonId,
                Trn = createPersonResult.Trn,
                DateOfBirth = createPersonResult.DateOfBirth,
                FirstName = createPersonResult.FirstName,
                MiddleName = createPersonResult.MiddleName,
                LastName = createPersonResult.LastName,
                EmailAddress = createPersonResult.Email,
                NationalInsuranceNumber = createPersonResult.NationalInsuranceNumber,
                CreatedOn = Clock.UtcNow,
                UpdatedOn = Clock.UtcNow,
            });
            await dbContext.SaveChangesAsync();

            return createPersonResult;
        });
    }

    public class CreatePersonBuilder
    {
        private DateOnly? _dateOfBirth;
        private bool? _hasTrn;
        private string? _firstName;
        private string? _middleName;
        private string? _lastName;
        private string? _email;
        private string? _mobileNumber;
        private bool? _hasNationalInsuranceNumber;
        private string? _nationalInsuranceNumber;

        public Guid PersonId { get; } = Guid.NewGuid();

        public CreatePersonBuilder WithDateOfBirth(DateOnly dateOfBirth)
        {
            if (_dateOfBirth is not null && _dateOfBirth != dateOfBirth)
            {
                throw new InvalidOperationException("WithDateOfBirth cannot be changed after it's set.");
            }

            _dateOfBirth = dateOfBirth;
            return this;
        }

        public CreatePersonBuilder WithFirstName(string firstName)
        {
            if (_firstName is not null && _firstName != firstName)
            {
                throw new InvalidOperationException("WithFirstName cannot be changed after it's set.");
            }

            _firstName = firstName;
            return this;
        }

        public CreatePersonBuilder WithMiddleName(string middleName)
        {
            if (_middleName is not null && _middleName != middleName)
            {
                throw new InvalidOperationException("WithMiddleName cannot be changed after it's set.");
            }

            _middleName = middleName;
            return this;
        }

        public CreatePersonBuilder WithLastName(string lastName)
        {
            if (_lastName is not null && _lastName != lastName)
            {
                throw new InvalidOperationException("WithLastName cannot be changed after it's set.");
            }

            _lastName = lastName;
            return this;
        }

        public CreatePersonBuilder WithEmail(string email)
        {
            if (_email is not null && _email != email)
            {
                throw new InvalidOperationException("WithEmail cannot be changed after it's set.");
            }

            _email = email;
            return this;
        }

        public CreatePersonBuilder WithMobileNumber(string mobileNumber)
        {
            if (_mobileNumber is not null && _mobileNumber != mobileNumber)
            {
                throw new InvalidOperationException("WithMobileNumber cannot be changed after it's set.");
            }

            _mobileNumber = mobileNumber;
            return this;
        }

        public CreatePersonBuilder WithTrn(bool hasTrn = true)
        {
            if (_hasTrn is not null && _hasTrn != hasTrn)
            {
                throw new InvalidOperationException("WithTrn cannot be changed after it's set.");
            }

            _hasTrn = hasTrn;
            return this;
        }

        public CreatePersonBuilder WithNationalInsuranceNumber(bool? hasNationalInsuranceNumber = true, string? nationalInsuranceNumber = null)
        {
            if ((_hasNationalInsuranceNumber is not null && _hasNationalInsuranceNumber != hasNationalInsuranceNumber)
                || (_nationalInsuranceNumber is not null && _nationalInsuranceNumber != nationalInsuranceNumber))
            {
                throw new InvalidOperationException("WithNationalInsuranceNumber cannot be changed after it's set.");
            }

            _hasNationalInsuranceNumber = hasNationalInsuranceNumber;
            _nationalInsuranceNumber = nationalInsuranceNumber;
            return this;
        }

        public CreatePersonBuilder WithNationalInsuranceNumber(string nationalInsuranceNumber)
        {
            var hasNationalInsuranceNumber = true;

            if ((_hasNationalInsuranceNumber is not null && _hasNationalInsuranceNumber != hasNationalInsuranceNumber)
                || (_nationalInsuranceNumber is not null && _nationalInsuranceNumber != nationalInsuranceNumber))
            {
                throw new InvalidOperationException("WithNationalInsuranceNumber cannot be changed after it's set.");
            }

            _hasNationalInsuranceNumber = hasNationalInsuranceNumber;
            _nationalInsuranceNumber = nationalInsuranceNumber;
            return this;
        }

        internal async Task<CreatePersonResult> Execute(TestData testData)
        {
            var hasTrn = _hasTrn ?? true;
            var trn = hasTrn ? await testData.GenerateTrn() : null;
            var hasNationalInsuranceNumber = _hasNationalInsuranceNumber ?? false;
            var nationalInsuranceNumber = hasNationalInsuranceNumber ? _nationalInsuranceNumber ?? testData.GenerateNationalInsuranceNumber() : null;
            var statedFirstName = _firstName ?? testData.GenerateFirstName();
            var statedMiddleName = _middleName ?? testData.GenerateMiddleName();
            var firstAndMiddleNames = $"{statedFirstName} {statedMiddleName}".Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var firstName = firstAndMiddleNames.First();
            var middleName = string.Join(" ", firstAndMiddleNames.Skip(1));
            var lastName = _lastName ?? testData.GenerateLastName();
            var dateOfBirth = _dateOfBirth ?? testData.GenerateDateOfBirth();

            return new CreatePersonResult()
            {
                PersonId = PersonId,
                Trn = trn,
                DateOfBirth = dateOfBirth,
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                Email = _email,
                NationalInsuranceNumber = nationalInsuranceNumber,
            };
        }
    }

    public record CreatePersonResult
    {
        public required Guid PersonId { get; init; }
        public required string? Trn { get; init; }
        public required DateOnly DateOfBirth { get; init; }
        public required string FirstName { get; init; }
        public required string MiddleName { get; init; }
        public required string LastName { get; init; }
        public required string? Email { get; init; }
        public required string? NationalInsuranceNumber { get; init; }
    }
}
