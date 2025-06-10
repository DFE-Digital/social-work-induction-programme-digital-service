using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

public interface ICreateAccountJourneyService
{
    IList<AccountType>? GetAccountTypes();

    AccountDetails? GetAccountDetails();

    bool? GetIsStaff();

    string GetIsStaffLabel();

    void SetAccountDetails(AccountDetails accountDetails);

    void SetAccountTypes(IList<AccountType> accountTypes);

    void SetIsStaff(bool? isStaff);

    void SetExternalUserId(int? externalUserId);

    bool? GetIsRegisteredWithSocialWorkEngland();

    void SetIsRegisteredWithSocialWorkEngland(bool? isRegisteredWithSocialWorkEngland);

    string? GetIsRegisteredWithSocialWorkEnglandLabel();

    bool? GetIsStatutoryWorker();

    void SetIsStatutoryWorker(bool? isStatutoryWorker);

    string? GetIsStatutoryWorkerLabel();

    bool? GetIsAgencyWorker();

    void SetIsAgencyWorker(bool? isAgencyWorker);

    string? GetIsAgencyWorkerLabel();

    bool? GetIsQualifiedWithin3Years();

    void SetIsQualifiedWithin3Years(bool? isQualifiedWithin3Years);

    string? GetIsQualifiedWithin3YearsLabel();

    Task<Account> CompleteJourneyAsync();

    void ResetCreateAccountJourneyModel();

    void SetSocialWorkerDetails(SocialWorker socialWorkerDetails);

    SocialWorker? GetSocialWorkerDetails();
}
