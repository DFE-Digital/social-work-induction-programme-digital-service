namespace Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;

public class RegisterSocialWorkerJourneyModel(Account account)
{
    public Account Account { get; } = account;

    public DateOnly? DateOfBirth { get; set; } = account.DateOfBirth;

    public UserSex? UserSex { get; set; } = account.UserSex;

    public GenderMatchesSexAtBirth? GenderMatchesSexAtBirth { get; set; } = account.GenderMatchesSexAtBirth;

    public string? OtherGenderIdentity { get; set; } = account.OtherGenderIdentity;

    public EthnicGroup? EthnicGroup { get; set; } = account.EthnicGroup;

    public EthnicGroupWhite? EthnicGroupWhite { get; set; } = account.EthnicGroupWhite;

    public string? OtherEthnicGroupWhite { get; set; } = account.OtherEthnicGroupWhite;

    public EthnicGroupMixed? EthnicGroupMixed { get; set; } = account.EthnicGroupMixed;

    public string? OtherEthnicGroupMixed { get; set; } = account.OtherEthnicGroupMixed;

    public EthnicGroupAsian? EthnicGroupAsian { get; set; } = account.EthnicGroupAsian;

    public string? OtherEthnicGroupAsian { get; set; } = account.OtherEthnicGroupAsian;

    public EthnicGroupBlack? EthnicGroupBlack { get; set; } = account.EthnicGroupBlack;

    public string? OtherEthnicGroupBlack { get; set; } = account.OtherEthnicGroupBlack;

    public EthnicGroupOther? EthnicGroupOther { get; set; } = account.EthnicGroupOther;

    public string? OtherEthnicGroupOther { get; set; } = account.OtherEthnicGroupOther;

    public Disability? Disability { get; set; } = account.Disability;

    public DateOnly? SocialWorkEnglandRegistrationDate { get; set; } = account.SocialWorkEnglandRegistrationDate;

    public Qualification? HighestQualification { get; set; } = account.HighestQualification;

    public int? SocialWorkQualificationEndYear { get; set; } = account.SocialWorkQualificationEndYear;

    public RouteIntoSocialWork? RouteIntoSocialWork { get; set; } = account.RouteIntoSocialWork;

    public string? OtherRouteIntoSocialWork { get; set; } = account.OtherRouteIntoSocialWork;

    public Account ToAccount()
    {
        return new Account(Account)
        {
            DateOfBirth = DateOfBirth,
            UserSex = UserSex,
            GenderMatchesSexAtBirth = GenderMatchesSexAtBirth,
            OtherGenderIdentity = OtherGenderIdentity,
            EthnicGroup = EthnicGroup,
            EthnicGroupWhite = EthnicGroupWhite,
            OtherEthnicGroupWhite = OtherEthnicGroupWhite,
            EthnicGroupMixed = EthnicGroupMixed,
            OtherEthnicGroupMixed = OtherEthnicGroupMixed,
            EthnicGroupAsian = EthnicGroupAsian,
            OtherEthnicGroupAsian = OtherEthnicGroupAsian,
            EthnicGroupBlack = EthnicGroupBlack,
            OtherEthnicGroupBlack = OtherEthnicGroupBlack,
            EthnicGroupOther = EthnicGroupOther,
            OtherEthnicGroupOther = OtherEthnicGroupOther,
            Disability = Disability,
            SocialWorkEnglandRegistrationDate = SocialWorkEnglandRegistrationDate,
            HighestQualification = HighestQualification,
            SocialWorkQualificationEndYear = SocialWorkQualificationEndYear,
            RouteIntoSocialWork = RouteIntoSocialWork,
            OtherRouteIntoSocialWork = OtherRouteIntoSocialWork
        };
    }
}
