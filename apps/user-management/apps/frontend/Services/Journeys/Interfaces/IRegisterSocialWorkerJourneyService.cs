using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

public interface IRegisterSocialWorkerJourneyService
{
    public IEthnicGroupService EthnicGroups { get; init; }

    Task<DateOnly?> GetDateOfBirthAsync(Guid accountId);
    Task SetDateOfBirthAsync(Guid accountId, DateOnly? dateOfBirth);
    Task<UserSex?> GetUserSexAsync(Guid accountId);
    Task SetUserSexAsync(Guid accountId, UserSex? userSex);
    Task<GenderMatchesSexAtBirth?> GetUserGenderMatchesSexAtBirthAsync(Guid accountId);
    Task SetUserGenderMatchesSexAtBirthAsync(Guid accountId, GenderMatchesSexAtBirth? genderMatchesSexAtBirth);
    Task<string?> GetOtherGenderIdentityAsync(Guid accountId);
    Task SetOtherGenderIdentityAsync(Guid accountId, string? otherGenderIdentity);
    Task<Disability?> GetIsDisabledAsync(Guid accountId);
    Task SetIsDisabledAsync(Guid accountId, Disability? isDisabled);
    Task<DateOnly?> GetSocialWorkEnglandRegistrationDateAsync(Guid accountId);
    Task SetSocialWorkEnglandRegistrationDateAsync(Guid accountId, DateOnly? socialWorkEnglandRegistrationDate);
    Task<Qualification?> GetHighestQualificationAsync(Guid accountId);
    Task SetHighestQualificationAsync(Guid accountId, Qualification? userSex);
    Task<int?> GetSocialWorkQualificationEndYearAsync(Guid accountId);
    Task SetSocialWorkQualificationEndYearAsync(Guid accountId, int? socialWorkEnglandQualificationEndYear);
    void ResetRegisterSocialWorkerJourneyModel(Guid accountId);
    Task<Account> CompleteJourneyAsync(Guid accountId);
}
