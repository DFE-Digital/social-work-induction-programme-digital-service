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
            var newPerson = new Person
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
                PersonOrganisations = createPersonResult.PersonOrganisations,
                ExternalUserId = createPersonResult.ExternalUserId,
                IsFunded = createPersonResult.IsFunded,
                ProgrammeStartDate = createPersonResult.ProgrammeStartDate,
                ProgrammeEndDate = createPersonResult.ProgrammeEndDate
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
        private string? _email;
        private int _externalUserId;
        private string? _firstName;
        private bool? _hasNationalInsuranceNumber;
        private bool? _hasTrn;
        private bool _isFunded;
        private string? _lastName;
        private string? _middleName;
        private string? _mobileNumber;
        private string? _nationalInsuranceNumber;
        private Guid _organisationId;
        private DateOnly? _programmeEndDate;
        private DateOnly? _programmeStartDate;
        private PersonStatus? _status;

        private Guid PersonId { get; } = Guid.NewGuid();

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

        public CreatePersonBuilder WithExternalUserId(int externalUserId)
        {
            if (_externalUserId != externalUserId)
            {
                throw new InvalidOperationException("WithExternalUserId cannot be changed after it's set.");
            }

            _externalUserId = externalUserId;
            return this;
        }

        public CreatePersonBuilder WithIsFunded(bool isFunded)
        {
            if (_isFunded != isFunded)
            {
                throw new InvalidOperationException("WithIsFunded cannot be changed after it's set.");
            }

            _isFunded = isFunded;
            return this;
        }

        public CreatePersonBuilder WithProgrammeStartDate(DateOnly programmeStartDate)
        {
            if (_programmeStartDate is not null && _programmeStartDate != programmeStartDate)
            {
                throw new InvalidOperationException(
                    "WithProgrammeStartDate cannot be changed after it's set."
                );
            }

            _programmeStartDate = programmeStartDate;
            return this;
        }

        public CreatePersonBuilder WithProgrammeEndDate(DateOnly programmeEndDate)
        {
            if (_programmeEndDate is not null && _programmeEndDate != programmeEndDate)
            {
                throw new InvalidOperationException(
                    "WithProgrammeEndDate cannot be changed after it's set."
                );
            }

            _programmeEndDate = programmeEndDate;
            return this;
        }

        internal async Task<CreatePersonResult> Execute(TestData testData)
        {
            var hasTrn = _hasTrn ?? true;
            var trn = hasTrn ? await testData.GenerateTrn() : null;
            var hasNationalInsuranceNumber = _hasNationalInsuranceNumber ?? false;
            var nationalInsuranceNumber = hasNationalInsuranceNumber
                ? _nationalInsuranceNumber ?? GenerateNationalInsuranceNumber()
                : null;
            var statedFirstName = _firstName ?? GenerateFirstName();
            var statedMiddleName = _middleName ?? GenerateMiddleName();
            var firstAndMiddleNames = $"{statedFirstName} {statedMiddleName}".Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );
            var firstName = firstAndMiddleNames.First();
            var middleName = string.Join(" ", firstAndMiddleNames.Skip(1));
            var lastName = _lastName ?? GenerateLastName();
            var dateOfBirth = _dateOfBirth ?? GenerateDateOfBirth();
            var status = _status ?? PersonStatus.Active;
            var personOrganisations = _organisationId != Guid.Empty
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
            const bool isFunded = true;
            var programmeStartDate = _programmeStartDate ?? GenerateDate(DateOnly.FromDateTime(DateTime.Now));
            var programmeEndDate = _programmeEndDate ?? GenerateDate(programmeStartDate);

            return new CreatePersonResult
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
                PersonOrganisations = personOrganisations,
                IsFunded = isFunded,
                ProgrammeStartDate = programmeStartDate,
                ProgrammeEndDate = programmeEndDate
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
        public int? ExternalUserId { get; set; }
        public bool IsFunded { get; init; }
        public DateOnly? ProgrammeStartDate { get; init; }
        public DateOnly? ProgrammeEndDate { get; init; }

        public Person ToPerson()
        {
            return new Person
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
                ExternalUserId = ExternalUserId,
                IsFunded = IsFunded,
                ProgrammeStartDate = ProgrammeStartDate,
                ProgrammeEndDate = ProgrammeEndDate
            };
        }

        public PersonDto ToPersonDto()
        {
            return ToPerson().ToDto();
        }
    }
}
