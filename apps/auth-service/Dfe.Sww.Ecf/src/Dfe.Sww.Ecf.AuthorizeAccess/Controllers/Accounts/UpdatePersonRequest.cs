using System.Collections.Immutable;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.RegisterSocialWorker;
using JetBrains.Annotations;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Accounts;

[PublicAPI]
public record UpdatePersonRequest
{
    public required Guid PersonId { get; set; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string? EmailAddress { get; init; }
    public string? SocialWorkEnglandNumber { get; init; }
    public PersonStatus? Status { get; init; }
    public ImmutableList<RoleType> Roles { get; init; } = [];
    public DateOnly? ProgrammeStartDate { get; init; }
    public DateOnly? ProgrammeEndDate { get; init; }

    // ECSW Registration Questions
    public DateOnly? DateOfBirth { get; init; }

    public UserSex? UserSex { get; init; }

    public GenderMatchesSexAtBirth? GenderMatchesSexAtBirth { get; init; }

    public string? OtherGenderIdentity { get; init; }

    public EthnicGroup? EthnicGroup { get; init; }

    public EthnicGroupWhite? EthnicGroupWhite { get; init; }

    public string? OtherEthnicGroupWhite { get; init; }

    public EthnicGroupAsian? EthnicGroupAsian { get; init; }

    public string? OtherEthnicGroupAsian { get; init; }

    public EthnicGroupMixed? EthnicGroupMixed { get; init; }

    public string? OtherEthnicGroupMixed { get; init; }

    public EthnicGroupBlack? EthnicGroupBlack { get; init; }

    public string? OtherEthnicGroupBlack { get; init; }

    public EthnicGroupOther? EthnicGroupOther { get; init; }

    public string? OtherEthnicGroupOther { get; init; }

    public Disability? Disability { get; init; }

    public DateOnly? SocialWorkEnglandRegistrationDate { get; init; }

    public Qualification? HighestQualification { get; init; }

    public RouteIntoSocialWork? RouteIntoSocialWork { get; init; }

    public string? OtherRouteIntoSocialWork { get; init; }

    public int? SocialWorkQualificationEndYear { get; set; }
}

public static class UpdatePersonRequestExtensions
{
    public static Person ToPerson(this UpdatePersonRequest request) =>
        new()
        {
            PersonId = request.PersonId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailAddress = request.EmailAddress,
            Trn = request.SocialWorkEnglandNumber,
            PersonRoles = request
                .Roles.Select(roleType => new PersonRole { RoleId = (int)roleType })
                .ToList(),
            Status = request.Status,
            ProgrammeStartDate = request.ProgrammeStartDate,
            ProgrammeEndDate = request.ProgrammeEndDate,
            DateOfBirth = request.DateOfBirth,
            UserSex = request.UserSex,
            GenderMatchesSexAtBirth = request.GenderMatchesSexAtBirth,
            OtherGenderIdentity = request.OtherGenderIdentity,
            EthnicGroup = request.EthnicGroup,
            EthnicGroupWhite = request.EthnicGroupWhite,
            OtherEthnicGroupWhite = request.OtherEthnicGroupWhite,
            EthnicGroupAsian = request.EthnicGroupAsian,
            OtherEthnicGroupAsian = request.OtherEthnicGroupAsian,
            EthnicGroupMixed = request.EthnicGroupMixed,
            OtherEthnicGroupMixed = request.OtherEthnicGroupMixed,
            EthnicGroupBlack = request.EthnicGroupBlack,
            OtherEthnicGroupBlack = request.OtherEthnicGroupBlack,
            EthnicGroupOther = request.EthnicGroupOther,
            OtherEthnicGroupOther = request.OtherEthnicGroupOther,
            Disability = request.Disability,
            SocialWorkEnglandRegistrationDate = request.SocialWorkEnglandRegistrationDate,
            HighestQualification = request.HighestQualification,
            RouteIntoSocialWork = request.RouteIntoSocialWork,
            OtherRouteIntoSocialWork = request.OtherRouteIntoSocialWork,
            SocialWorkQualificationEndYear = request.SocialWorkQualificationEndYear
        };
}
