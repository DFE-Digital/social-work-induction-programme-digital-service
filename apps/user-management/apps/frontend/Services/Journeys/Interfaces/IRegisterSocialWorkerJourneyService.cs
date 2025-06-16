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
    Task<EthnicGroupWhite?> GetWhiteEthnicGroupAsync(Guid accountId);
    Task SetEthnicGroupWhiteAsync(Guid accountId, EthnicGroupWhite? ethnicGroupWhite);
    Task<string?> GetOtherWhiteEthnicGroupAsync(Guid accountId);
    Task SetOtherWhiteEthnicGroupAsync(Guid accountId, string? otherWhiteEthnicGroup);
    Task<EthnicGroupMixed?> GetMixedEthnicGroupAsync(Guid accountId);
    Task SetEthnicGroupMixedAsync(Guid accountId, EthnicGroupMixed? ethnicGroupMixed);
    Task<string?> GetOtherMixedEthnicGroupAsync(Guid accountId);
    Task SetOtherMixedEthnicGroupAsync(Guid accountId, string? otherMixedEthnicGroup);
    void ResetRegisterSocialWorkerJourneyModel(Guid accountId);
    Task<Account> CompleteJourneyAsync(Guid accountId);
}
