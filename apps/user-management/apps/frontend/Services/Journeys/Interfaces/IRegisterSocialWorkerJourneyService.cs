using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;

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
    Task<bool?> GetIsDisabledAsync(Guid accountId);
    Task SetIsDisabledAsync(Guid accountId, bool? isDisabled);

    public IEthnicGroupService EthnicGroups { get; init; }

    void ResetRegisterSocialWorkerJourneyModel(Guid accountId);
    Task<Account> CompleteJourneyAsync(Guid accountId);
}
