using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Services.Accounts;

namespace Dfe.Sww.Ecf.TestCommon;

public partial class TestData
{
    public Task<CreatePersonResult> CreatePerson(
        Action<CreatePersonBuilder>? configure = null,
        bool? addToDb = true
    )
    {
        return WithDbContext(async dbContext =>
        {
            var builder = new CreatePersonBuilder();
            configure?.Invoke(builder);

            var createPersonResult = await builder.Execute(this);
            var newPerson = new Person()
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
                Status = createPersonResult.Status,
                PersonOrganisations = createPersonResult.PersonOrganisations
            };

            if (addToDb is not true)
            {
                return createPersonResult;
            }

            dbContext.Persons.Add(newPerson);
            await dbContext.SaveChangesAsync();

            return createPersonResult;
        });
    }

    public async Task<CreatePersonResult[]> CreatePersons(int count, Guid organisationId)
    {
        var results = new CreatePersonResult[count];

        for (var i = 0; i < count; i++)
        {
            results[i] = await CreatePerson(b => b.WithOrganisationId(organisationId));
        }

        return results.ToArray();
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
        private PersonStatus? _status;
        private Guid _organisationId;

        public Guid PersonId { get; } = Guid.NewGuid();

        public CreatePersonBuilder WithDateOfBirth(DateOnly dateOfBirth)
        {
            if (_dateOfBirth is not null && _dateOfBirth != dateOfBirth)
            {
                throw new InvalidOperationException(
                    "WithDateOfBirth cannot be changed after it's set."
                );
            }

            _dateOfBirth = dateOfBirth;
            return this;
        }

        public CreatePersonBuilder WithFirstName(string firstName)
        {
            if (_firstName is not null && _firstName != firstName)
            {
                throw new InvalidOperationException(
                    "WithFirstName cannot be changed after it's set."
                );
            }

            _firstName = firstName;
            return this;
        }

        public CreatePersonBuilder WithMiddleName(string middleName)
        {
            if (_middleName is not null && _middleName != middleName)
            {
                throw new InvalidOperationException(
                    "WithMiddleName cannot be changed after it's set."
                );
            }

            _middleName = middleName;
            return this;
        }

        public CreatePersonBuilder WithLastName(string lastName)
        {
            if (_lastName is not null && _lastName != lastName)
            {
                throw new InvalidOperationException(
                    "WithLastName cannot be changed after it's set."
                );
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
                throw new InvalidOperationException(
                    "WithMobileNumber cannot be changed after it's set."
                );
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

        public CreatePersonBuilder WithNationalInsuranceNumber(
            bool? hasNationalInsuranceNumber = true,
            string? nationalInsuranceNumber = null
        )
        {
            if (
                (
                    _hasNationalInsuranceNumber is not null
                    && _hasNationalInsuranceNumber != hasNationalInsuranceNumber
                )
                || (
                    _nationalInsuranceNumber is not null
                    && _nationalInsuranceNumber != nationalInsuranceNumber
                )
            )
            {
                throw new InvalidOperationException(
                    "WithNationalInsuranceNumber cannot be changed after it's set."
                );
            }

            _hasNationalInsuranceNumber = hasNationalInsuranceNumber;
            _nationalInsuranceNumber = nationalInsuranceNumber;
            return this;
        }

        public CreatePersonBuilder WithStatus(PersonStatus status)
        {
            if (_status is not null && _status != status)
            {
                throw new InvalidOperationException("WithStatus cannot be changed after it's set.");
            }

            _status = status;
            return this;
        }

        public CreatePersonBuilder WithNationalInsuranceNumber(string nationalInsuranceNumber)
        {
            var hasNationalInsuranceNumber = true;

            if (
                (
                    _hasNationalInsuranceNumber is not null
                    && _hasNationalInsuranceNumber != hasNationalInsuranceNumber
                )
                || (
                    _nationalInsuranceNumber is not null
                    && _nationalInsuranceNumber != nationalInsuranceNumber
                )
            )
            {
                throw new InvalidOperationException(
                    "WithNationalInsuranceNumber cannot be changed after it's set."
                );
            }

            _hasNationalInsuranceNumber = hasNationalInsuranceNumber;
            _nationalInsuranceNumber = nationalInsuranceNumber;
            return this;
        }

        public CreatePersonBuilder WithOrganisationId(Guid organisationId)
        {
            if (_organisationId != Guid.Empty && _organisationId != organisationId)
            {
                throw new InvalidOperationException("WithOrganisationId cannot be changed after it's set.");
            }

            _organisationId = organisationId;
            return this;
        }

        internal async Task<CreatePersonResult> Execute(TestData testData)
        {
            var hasTrn = _hasTrn ?? true;
            var trn = hasTrn ? await testData.GenerateTrn() : null;
            var hasNationalInsuranceNumber = _hasNationalInsuranceNumber ?? false;
            var nationalInsuranceNumber = hasNationalInsuranceNumber
                ? _nationalInsuranceNumber ?? testData.GenerateNationalInsuranceNumber()
                : null;
            var statedFirstName = _firstName ?? testData.GenerateFirstName();
            var statedMiddleName = _middleName ?? testData.GenerateMiddleName();
            var firstAndMiddleNames = $"{statedFirstName} {statedMiddleName}".Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );
            var firstName = firstAndMiddleNames.First();
            var middleName = string.Join(" ", firstAndMiddleNames.Skip(1));
            var lastName = _lastName ?? testData.GenerateLastName();
            var dateOfBirth = _dateOfBirth ?? testData.GenerateDateOfBirth();
            var status = _status ?? PersonStatus.Active;
            var personOrganisations = (_organisationId != Guid.Empty)
                ?
                [
                    new PersonOrganisation
                    {
                        OrganisationId = _organisationId,
                        PersonId = PersonId,
                        PersonOrganisationId = Guid.NewGuid()
                    }
                ]
                : new List<PersonOrganisation>();
            ;

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
                CreatedOn = testData.Clock.UtcNow,
                Status = status,
                PersonOrganisations = personOrganisations
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
        public string? Email { get; init; }
        public string? NationalInsuranceNumber { get; init; }
        public DateTime? CreatedOn { get; init; }
        public DateTime? UpdatedOn { get; init; }
        public PersonStatus Status { get; init; }
        public List<PersonOrganisation> PersonOrganisations { get; init; } = [];

        public Person ToPerson() =>
            new Person
            {
                PersonId = PersonId,
                Trn = Trn,
                DateOfBirth = DateOfBirth,
                FirstName = FirstName,
                MiddleName = MiddleName,
                LastName = LastName,
                EmailAddress = Email,
                NationalInsuranceNumber = NationalInsuranceNumber,
                CreatedOn = CreatedOn,
                UpdatedOn = UpdatedOn,
                Status = Status,
            };

        public PersonDto ToPersonDto() => ToPerson().ToDto();
    }
}
