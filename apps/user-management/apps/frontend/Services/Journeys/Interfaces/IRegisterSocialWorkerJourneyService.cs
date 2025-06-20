using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

public interface IRegisterSocialWorkerJourneyService
{
    public IEthnicGroupService EthnicGroups { get; init; }

    public Task<RegisterSocialWorkerJourneyModel?> GetRegisterSocialWorkerJourneyModelAsync(Guid personId);

    Task<DateOnly?> GetDateOfBirthAsync(Guid personId);
    Task SetDateOfBirthAsync(Guid personId, DateOnly? dateOfBirth);
    Task<UserSex?> GetUserSexAsync(Guid personId);
    Task SetUserSexAsync(Guid personId, UserSex? userSex);
    Task<GenderMatchesSexAtBirth?> GetUserGenderMatchesSexAtBirthAsync(Guid personId);
    Task SetUserGenderMatchesSexAtBirthAsync(Guid personId, GenderMatchesSexAtBirth? genderMatchesSexAtBirth);
    Task<string?> GetOtherGenderIdentityAsync(Guid personId);
    Task SetOtherGenderIdentityAsync(Guid personId, string? otherGenderIdentity);
    Task<Disability?> GetIsDisabledAsync(Guid personId);
    Task SetIsDisabledAsync(Guid personId, Disability? isDisabled);
    Task<DateOnly?> GetSocialWorkEnglandRegistrationDateAsync(Guid personId);
    Task SetSocialWorkEnglandRegistrationDateAsync(Guid personId, DateOnly? socialWorkEnglandRegistrationDate);
    Task<Qualification?> GetHighestQualificationAsync(Guid personId);
    Task SetHighestQualificationAsync(Guid personId, Qualification? userSex);
    Task<int?> GetSocialWorkQualificationEndYearAsync(Guid personId);
    Task SetSocialWorkQualificationEndYearAsync(Guid personId, int? socialWorkEnglandQualificationEndYear);
    Task<RouteIntoSocialWork?> GetRouteIntoSocialWorkAsync(Guid personId);
    Task SetRouteIntoSocialWorkAsync(Guid personId, RouteIntoSocialWork? routeIntoSocialWork);
    Task<string?> GetOtherRouteIntoSocialWorkAsync(Guid personId);
    Task SetOtherRouteIntoSocialWorkAsync(Guid personId, string? otherRouteIntoSocialWork);
    void ResetRegisterSocialWorkerJourneyModel(Guid personId);
    Task<Account> CompleteJourneyAsync(Guid personId);
}
