using SocialWorkInductionProgramme.Frontend.Models;
using SocialWorkInductionProgramme.Frontend.Services.Interfaces;
using SocialWorkInductionProgramme.Frontend.Services.Journeys.Interfaces;

namespace SocialWorkInductionProgramme.Frontend.Test.UnitTests.Extensions;

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
