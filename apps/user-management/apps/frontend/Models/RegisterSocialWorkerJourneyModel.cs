namespace Dfe.Sww.Ecf.Frontend.Models;

public class RegisterSocialWorkerJourneyModel(Account account)
{
    public Account Account { get; } = account;

    public DateOnly? DateOfBirth { get; set; } = account.DateOfBirth;

    public UserSex? UserSex { get; set; } = account.UserSex;

    public GenderMatchesSexAtBirth? GenderMatchesSexAtBirth { get; set; } = account.GenderMatchesSexAtBirth;

    public string? OtherGenderIdentity { get; set; } = account.OtherGenderIdentity;
    public EthnicGroup? EthnicGroup { get; set; } = account.EthnicGroup;

    public EthnicGroupWhite? EthnicGroupWhite { get; set; } = account.EthnicGroupWhite;

    public string? OtherWhiteEthnicGroup { get; set; } = account.OtherWhiteEthnicGroup;

    public EthnicGroupMixed? EthnicGroupMixed { get; set; } = account.EthnicGroupMixed;

    public string? OtherMixedEthnicGroup { get; set; } = account.OtherMixedEthnicGroup;

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
            OtherWhiteEthnicGroup = OtherWhiteEthnicGroup,
            EthnicGroupMixed = EthnicGroupMixed,
            OtherMixedEthnicGroup = OtherMixedEthnicGroup
        };
    }
}
