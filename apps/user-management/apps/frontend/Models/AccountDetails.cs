using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;

namespace Dfe.Sww.Ecf.Frontend.Models;

public class AccountDetails
{
    [Display(Name = "First name")] public string? FirstName { get; set; }

    [Display(Name = "Middle names")] public string? MiddleNames { get; set; }

    [Display(Name = "Last name")] public string? LastName { get; set; }

    [Display(Name = "Full name")]
    public string FullName => string.Join(" ",
        new[]
            {
                FirstName,
                MiddleNames,
                LastName
            }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

    [Display(Name = "Email address")] public string? Email { get; set; }

    public string? SocialWorkEnglandNumber { get; set; }

    public DateOnly? ProgrammeStartDate { get; set; }

    public DateOnly? ProgrammeEndDate { get; set; }

    public int? ExternalUserId { get; set; }

    public bool IsFunded { get; set; }

    public bool IsStaff { get; set; }

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

    public IList<AccountType>? Types { get; set; }

    [Display(Name = "UK phone number")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Set to true on pages where a phone number is needed
    /// </summary>
    public bool? PhoneNumberRequired { get; set; }

    public static AccountDetails FromAccount(Account account)
    {
        return new AccountDetails
        {
            FirstName = account.FirstName,
            MiddleNames = account.MiddleNames,
            LastName = account.LastName,
            Email = account.Email,
            SocialWorkEnglandNumber = account.SocialWorkEnglandNumber,
            ProgrammeStartDate = account.ProgrammeStartDate,
            ProgrammeEndDate = account.ProgrammeEndDate,
            ExternalUserId = account.ExternalUserId,
            IsFunded = account.IsFunded,
            IsStaff = account.IsStaff,
            DateOfBirth = account.DateOfBirth,
            UserSex = account.UserSex,
            GenderMatchesSexAtBirth = account.GenderMatchesSexAtBirth,
            OtherGenderIdentity = account.OtherGenderIdentity,
            EthnicGroup = account.EthnicGroup,
            EthnicGroupWhite = account.EthnicGroupWhite,
            OtherEthnicGroupWhite = account.OtherEthnicGroupWhite,
            EthnicGroupMixed = account.EthnicGroupMixed,
            OtherEthnicGroupMixed = account.OtherEthnicGroupMixed,
            EthnicGroupAsian = account.EthnicGroupAsian,
            OtherEthnicGroupAsian = account.OtherEthnicGroupAsian,
            EthnicGroupBlack = account.EthnicGroupBlack,
            OtherEthnicGroupBlack = account.OtherEthnicGroupBlack,
            EthnicGroupOther = account.EthnicGroupOther,
            OtherEthnicGroupOther = account.OtherEthnicGroupOther,
            Disability = account.Disability,
            SocialWorkEnglandRegistrationDate = account.SocialWorkEnglandRegistrationDate,
            HighestQualification = account.HighestQualification,
            SocialWorkQualificationEndYear = account.SocialWorkQualificationEndYear,
            RouteIntoSocialWork = account.RouteIntoSocialWork,
            OtherRouteIntoSocialWork = account.OtherRouteIntoSocialWork,
            Types = account.Types,
            PhoneNumber = account.PhoneNumber,
            PhoneNumberRequired = account.PhoneNumberRequired
        };
    }
}
