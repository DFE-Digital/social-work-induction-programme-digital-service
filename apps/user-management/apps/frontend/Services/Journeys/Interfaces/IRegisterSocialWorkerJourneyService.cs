using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

public interface IRegisterSocialWorkerJourneyService
{
    Task<DateOnly?> GetDateOfBirthAsync(Guid accountId);
    Task SetDateOfBirthAsync(Guid accountId, DateOnly? dateOfBirth);
    Task<UserSex?> GetUserSexAsync(Guid accountId);
    Task SetUserSexAsync(Guid accountId, UserSex? userSex);
    Task<GenderMatchesSexAtBirth?> GetUserGenderMatchesSexAtBirthAsync(Guid accountId);
    Task SetUserGenderMatchesSexAtBirthAsync(Guid accountId, GenderMatchesSexAtBirth? genderMatchesSexAtBirth);
    Task<string?> GetOtherGenderIdentityAsync(Guid accountId);
    Task SetOtherGenderIdentityAsync(Guid accountId, string? otherGenderIdentity);
    Task<EthnicGroup?> GetEthnicGroupAsync(Guid accountId);
    Task SetEthnicGroupAsync(Guid accountId, EthnicGroup? ethnicGroup);
    Task<EthnicGroupWhite?> GetEthnicGroupWhiteAsync(Guid accountId);
    Task SetEthnicGroupWhiteAsync(Guid accountId, EthnicGroupWhite? ethnicGroupWhite);
    Task<string?> GetOtherEthnicGroupWhiteAsync(Guid accountId);
    Task SetOtherEthnicGroupWhiteAsync(Guid accountId, string? otherEthnicGroupWhite);
    Task<EthnicGroupMixed?> GetEthnicGroupMixedAsync(Guid accountId);
    Task SetEthnicGroupMixedAsync(Guid accountId, EthnicGroupMixed? ethnicGroupMixed);
    Task<string?> GetOtherEthnicGroupMixedAsync(Guid accountId);
    Task SetOtherEthnicGroupMixedAsync(Guid accountId, string? otherEthnicGroupMixed);
    Task<EthnicGroupAsian?> GetEthnicGroupAsianAsync(Guid accountId);
    Task SetEthnicGroupAsianAsync(Guid accountId, EthnicGroupAsian? ethnicGroupAsian);
    Task<string?> GetOtherEthnicGroupAsianAsync(Guid accountId);
    Task SetOtherEthnicGroupAsianAsync(Guid accountId, string? otherEthnicGroupAsian);
    Task<EthnicGroupBlack?> GetEthnicGroupBlackAsync(Guid accountId);
    Task SetEthnicGroupBlackAsync(Guid accountId, EthnicGroupBlack? ethnicGroupBlack);
    Task<string?> GetOtherEthnicGroupBlackAsync(Guid accountId);
    Task SetOtherEthnicGroupBlackAsync(Guid accountId, string? otherEthnicGroupBlack);
    void ResetRegisterSocialWorkerJourneyModel(Guid accountId);
    Task<Account> CompleteJourneyAsync(Guid accountId);
}
