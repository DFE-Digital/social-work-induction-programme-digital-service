using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;
using JetBrains.Annotations;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;

[PublicAPI]
public record UpdatePersonRequest
{
    public required Guid PersonId { get; set; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string EmailAddress { get; init; }
    public string? SocialWorkEnglandNumber { get; init; }
    public AccountStatus? Status { get; init; }
    public ImmutableList<AccountType> Roles { get; init; } = [];

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
