using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.RegisterSocialWorker;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

public class Person
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid PersonId { get; init; }

    public int? ExternalUserId { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime? CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }
    public DateTime? DeletedOn { get; set; }

    [Required] [MaxLength(100)] public string FirstName { get; set; } = string.Empty;
    [MaxLength(100)] public string? MiddleName { get; set; }
    [Required] [MaxLength(100)] public string LastName { get; set; } = string.Empty;

    [MaxLength(254)] public string? EmailAddress { get; set; }
    [MaxLength(9)] public string? NationalInsuranceNumber { get; set; }
    [MaxLength(7)] public string? Trn { get; set; }

    public PersonStatus? Status { get; set; }

    [Required] public bool IsFunded { get; set; }

    public DateOnly? ProgrammeStartDate { get; set; }
    public DateOnly? ProgrammeEndDate { get; set; }

    public DateOnly? DateOfBirth { get; set; }
    public UserSex? UserSex { get; set; }
    public GenderMatchesSexAtBirth? GenderMatchesSexAtBirth { get; set; }
    [MaxLength(100)] public string? OtherGenderIdentity { get; set; }

    public EthnicGroup? EthnicGroup { get; set; }
    public EthnicGroupWhite? EthnicGroupWhite { get; set; }
    [MaxLength(100)] public string? OtherEthnicGroupWhite { get; set; }
    public EthnicGroupAsian? EthnicGroupAsian { get; set; }
    [MaxLength(100)] public string? OtherEthnicGroupAsian { get; set; }
    public EthnicGroupMixed? EthnicGroupMixed { get; set; }
    [MaxLength(100)] public string? OtherEthnicGroupMixed { get; set; }
    public EthnicGroupBlack? EthnicGroupBlack { get; set; }
    [MaxLength(100)] public string? OtherEthnicGroupBlack { get; set; }
    public EthnicGroupOther? EthnicGroupOther { get; set; }
    [MaxLength(100)] public string? OtherEthnicGroupOther { get; set; }

    public Disability? Disability { get; set; }

    public DateOnly? SocialWorkEnglandRegistrationDate { get; set; }
    public Qualification? HighestQualification { get; set; }
    public RouteIntoSocialWork? RouteIntoSocialWork { get; set; }
    [MaxLength(100)] public string? OtherRouteIntoSocialWork { get; set; }
    public int? SocialWorkQualificationEndYear { get; set; }

    public ICollection<PersonRole> PersonRoles { get; set; } = new List<PersonRole>();
    public ICollection<PersonOrganisation> PersonOrganisations { get; set; } = new List<PersonOrganisation>();
}
