using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.RegisterSocialWorker;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

public class Person
{
    public Guid PersonId { get; init; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public DateTime? DeletedOn { get; set; }
    public string? Trn { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required string LastName { get; set; }
    public string? EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }
    public string? NationalInsuranceNumber { get; set; }
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

    public ICollection<PersonRole> PersonRoles { get; set; } = new List<PersonRole>();
    public ICollection<PersonOrganisation> PersonOrganisations { get; set; } = new List<PersonOrganisation>();
}
