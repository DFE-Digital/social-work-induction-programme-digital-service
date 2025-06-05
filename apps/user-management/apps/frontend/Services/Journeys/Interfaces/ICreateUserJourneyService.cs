using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

public interface ICreateUserJourneyService
{
    IList<UserType>? GetUserTypes();

    UserDetails? GetUserDetails();

    bool? GetIsStaff();

    void SetUserDetails(UserDetails userDetails);

    void SetUserTypes(IList<UserType> userTypes);

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

    Task<User> CompleteJourneyAsync();

    void ResetCreateUserJourneyModel();

    void SetSocialWorkerDetails(SocialWorker socialWorkerDetails);

    SocialWorker? GetSocialWorkerDetails();
}
