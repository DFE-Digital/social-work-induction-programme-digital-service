using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Extensions;

public static class CreateAccountJourneyServiceExtensions
{
    public static void PopulateJourneyModelFromAccount(
        this ICreateAccountJourneyService createAccountJourneyService,
        Account account
    )
    {
        createAccountJourneyService.ResetCreateAccountJourneyModel();

        createAccountJourneyService.SetAccountTypes(account.Types!);
        createAccountJourneyService.SetAccountDetails(AccountDetails.FromAccount(account));
        createAccountJourneyService.SetIsStaff(
            account.Types?.Intersect([AccountType.Assessor, AccountType.Coordinator]).Any()
        );
    }
}
