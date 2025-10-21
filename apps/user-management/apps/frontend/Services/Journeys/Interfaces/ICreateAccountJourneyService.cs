using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

public interface ICreateAccountJourneyService
{
    IList<AccountType>? GetAccountTypes();

    AccountDetails? GetAccountDetails();

    bool? GetIsStaff();

    void SetAccountDetails(AccountDetails accountDetails);

    void SetAccountTypes(IList<AccountType> accountTypes);

    void SetIsStaff(bool? isStaff);

    void SetExternalUserId(int? externalUserId);

    bool? GetIsRegisteredWithSocialWorkEngland();

    void SetIsRegisteredWithSocialWorkEngland(bool? isRegisteredWithSocialWorkEngland);

    bool? GetIsStatutoryWorker();

    void SetIsStatutoryWorker(bool? isStatutoryWorker);

    bool? GetIsAgencyWorker();

    void SetIsAgencyWorker(bool? isAgencyWorker);

    void SetIsEnrolledInAsye(bool? isEnrolledInAsye);

    /// <summary>
    ///     Get property capturing whether the user has recently completed their social work qualification.
    /// </summary>
    /// <returns>True if the user has qualified in the last 3 years, false otherwise.</returns>
    bool? GetIsRecentlyQualified();

    void SetIsRecentlyQualified(bool? isRecentlyQualified);

    void SetProgrammeStartDate(DateOnly programmeStartDate);

    DateOnly? GetProgrammeStartDate();

    void SetProgrammeEndDate(DateOnly programmeEndDate);

    DateOnly? GetProgrammeEndDate();

    Task<Account> CompleteJourneyAsync(Guid? organisationId = null);

    void ResetCreateAccountJourneyModel();

    void SetSocialWorkerDetails(SocialWorker socialWorkerDetails);

    SocialWorker? GetSocialWorkerDetails();

    AccountLabels? GetAccountLabels();

    AccountChangeLinks GetAccountChangeLinks(Guid? organisationId = null);
}
