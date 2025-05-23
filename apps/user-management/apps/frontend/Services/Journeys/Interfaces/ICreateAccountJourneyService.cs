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

    bool? GetIsQualifiedWithin3Years();

    void SetIsQualifiedWithin3Years(bool? isQualifiedWithin3Years);

    Task<Account> CompleteJourneyAsync();

    void ResetCreateAccountJourneyModel();

    void SetSocialWorkerDetails(SocialWorker socialWorkerDetails);

    SocialWorker? GetSocialWorkerDetails();
}
