using System.Collections.Immutable;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.RegisterSocialWorker;
using Hangfire.Annotations;

namespace Dfe.Sww.Ecf.Core.Services.Accounts;

[PublicAPI]
public class PersonDto
{
    public Guid PersonId { get; init; }
    public DateTime? CreatedOn { get; init; }
    public DateTime? UpdatedOn { get; init; }
    public string? SocialWorkEnglandNumber { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required string LastName { get; set; }
    public string? EmailAddress { get; set; }
    public ImmutableList<RoleType> Roles { get; set; } = [];
    public PersonStatus? Status { get; set; }
    public int? ExternalUserId { get; set; }
    public bool IsFunded { get; set; }
    public DateOnly? ProgrammeStartDate { get; set; }
    public DateOnly? ProgrammeEndDate { get; set; }
    public DateOnly? DateOfBirth { get; set; }

    public UserSex? UserSex { get; set; }

    public GenderMatchesSexAtBirth? GenderMatchesSexAtBirth { get; set; }

    public string? OtherGenderIdentity { get; set; }

    public EthnicGroup? EthnicGroup { get; set; }

    public EthnicGroupWhite? EthnicGroupWhite { get; set; }

    public string? OtherEthnicGroupWhite { get; set; }

    public EthnicGroupAsian? EthnicGroupAsian { get; set; }

    public string? OtherEthnicGroupAsian { get; set; }

    public EthnicGroupMixed? EthnicGroupMixed { get; set; }

    public string? OtherEthnicGroupMixed { get; set; }

    public EthnicGroupBlack? EthnicGroupBlack { get; set; }

    public string? OtherEthnicGroupBlack { get; set; }

    public EthnicGroupOther? EthnicGroupOther { get; set; }

    public string? OtherEthnicGroupOther { get; set; }

    public Disability? Disability { get; set; }

    public DateOnly? SocialWorkEnglandRegistrationDate { get; set; }

    public Qualification? HighestQualification { get; set; }

    public RouteIntoSocialWork? RouteIntoSocialWork { get; set; }

    public string? OtherRouteIntoSocialWork { get; set; }

    public int? SocialWorkQualificationEndYear { get; set; }
}

public static class PersonDtoExtensions
{
    public static PersonDto ToDto(this Person person) =>
        new()
        {
            PersonId = person.PersonId,
            CreatedOn = person.CreatedOn,
            UpdatedOn = person.UpdatedOn,
            SocialWorkEnglandNumber = person.Trn,
            FirstName = person.FirstName,
            MiddleName = person.MiddleName,
            LastName = person.LastName,
            EmailAddress = person.EmailAddress,
            Roles = person.PersonRoles.Select(x => x.Role.RoleName).ToImmutableList() ?? [],
            Status = person.Status,
            ExternalUserId = person.ExternalUserId,
            IsFunded = person.IsFunded,
            ProgrammeStartDate = person.ProgrammeStartDate,
            ProgrammeEndDate = person.ProgrammeEndDate
        };
}
