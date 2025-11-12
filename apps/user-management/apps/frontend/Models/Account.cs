using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;

namespace Dfe.Sww.Ecf.Frontend.Models;

/// <summary>
/// User account
/// </summary>
public record Account
{
    /// <summary>
    /// Account ID
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Date and time the account was created
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// First Name
    /// </summary>
    [Display(Name = "First name")]
    public string? FirstName { get; init; }

    /// <summary>
    /// Middle Names
    /// </summary>
    [Display(Name = "Middle names")]
    public string? MiddleNames { get; init; }

    /// <summary>
    /// Last Name
    /// </summary>
    [Display(Name = "Last name")]
    public string? LastName { get; init; }

    /// <summary>
    /// FullName
    /// </summary>
    [Display(Name = "Name")]
    public string FullName => string.Join(" ",
        new[]
            {
                FirstName,
                MiddleNames,
                LastName
            }
            .Where(s => !string.IsNullOrWhiteSpace(s))).Trim();

    /// <summary>
    /// Email
    /// </summary>
    [Display(Name = "Email address")]
    public string? Email { get; init; }

    /// <summary>
    /// Account status
    /// </summary>
    [Display(Name = "Status")]
    public AccountStatus? Status { get; set; }

    /// <summary>
    /// Account types
    /// </summary>
    [Display(Name = "Account type")]
    public ImmutableList<AccountType>? Types { get; init; }

    /// <summary>
    /// Social Work England number
    /// </summary>
    [Display(Name = "Social Work England number")]
    public string? SocialWorkEnglandNumber { get; init; }

    public DateOnly? ProgrammeStartDate { get; init; }

    public DateOnly? ProgrammeEndDate { get; init; }

    public int? ExternalUserId { get; set; }

    public bool IsFunded { get; init; }

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

    public bool IsStaff =>
        Types?.Any(t => t is AccountType.Coordinator or AccountType.Assessor) ?? false;

    public bool HasCompletedLoginAccountLinking { get; set; }

    [Display(Name = "UK phone number")]
    public string? PhoneNumber { get; set; }

    public bool? PhoneNumberRequired { get; set; }

    public Account(Account account)
    {
        Id = account.Id;
        CreatedAt = account.CreatedAt;
        FirstName = account.FirstName;
        MiddleNames = account.MiddleNames;
        LastName = account.LastName;
        Email = account.Email;
        Status = account.Status;
        Types = account.Types;
        SocialWorkEnglandNumber = account.SocialWorkEnglandNumber;
        ProgrammeStartDate = account.ProgrammeStartDate;
        ProgrammeEndDate = account.ProgrammeEndDate;
        ExternalUserId = account.ExternalUserId;
        IsFunded = account.IsFunded;
        DateOfBirth = account.DateOfBirth;
        UserSex = account.UserSex;
        GenderMatchesSexAtBirth = account.GenderMatchesSexAtBirth;
        OtherGenderIdentity = account.OtherGenderIdentity;
        EthnicGroup = account.EthnicGroup;
        EthnicGroupWhite = account.EthnicGroupWhite;
        OtherEthnicGroupWhite = account.OtherEthnicGroupWhite;
        EthnicGroupMixed = account.EthnicGroupMixed;
        OtherEthnicGroupMixed = account.OtherEthnicGroupMixed;
        EthnicGroupAsian = account.EthnicGroupAsian;
        OtherEthnicGroupAsian = account.OtherEthnicGroupAsian;
        EthnicGroupBlack = account.EthnicGroupBlack;
        OtherEthnicGroupBlack = account.OtherEthnicGroupBlack;
        EthnicGroupOther = account.EthnicGroupOther;
        OtherEthnicGroupOther = account.OtherEthnicGroupOther;
        Disability = account.Disability;
        SocialWorkEnglandRegistrationDate = account.SocialWorkEnglandRegistrationDate;
        HighestQualification = account.HighestQualification;
        SocialWorkQualificationEndYear = account.SocialWorkQualificationEndYear;
        RouteIntoSocialWork = account.RouteIntoSocialWork;
        OtherRouteIntoSocialWork = account.OtherRouteIntoSocialWork;
        HasCompletedLoginAccountLinking = account.HasCompletedLoginAccountLinking;
        PhoneNumber = account.PhoneNumber;
        PhoneNumberRequired = account.PhoneNumberRequired;
    }
}
