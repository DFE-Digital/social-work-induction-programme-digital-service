using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Views.Accounts;

namespace Dfe.Sww.Ecf.Frontend.Services.Interfaces;

public interface ICreateAccountJourneyService
{
    CreateAccountJourneyModel GetCreateAccountJourneyModel();

    SelectUserTypeModel? GetUserType();

    AddUserDetailsModel? GetUserDetails();

    void SetUserDetails(AddUserDetailsModel userDetails);

    void SetAccountType(SelectUserTypeModel userType);

    void ResetCreateAccountJourneyModel();

    Account CompleteJourney();
}
