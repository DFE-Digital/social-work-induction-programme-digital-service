using System.Diagnostics.CodeAnalysis;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.DAL;

namespace Dfe.Sww.Ecf.Frontend.Installers;

[ExcludeFromCodeCoverage]
public static class InstallRepositories
{
    public static void AddRepositories(this IServiceCollection services)
    {
        var accountsRepository = new AccountsRepository();
        services.AddSingleton(accountsRepository);

        // TODO: Remove this once the DAL is in place
        accountsRepository.AddRange(
        [
            new Account
            {
                FirstName = "Ellen",
                LastName = "Ripley",
                Status = AccountStatus.Paused,
                Types = [AccountType.EarlyCareerSocialWorker],
                Id = 0
            },
            new Account
            {
                FirstName = "Joe",
                LastName = "Bloggs",
                Status = AccountStatus.Active,
                Types = [AccountType.Coordinator],
                Id = 1
            },
            new Account
            {
                FirstName = "Laura",
                LastName = "Barn",
                Status = AccountStatus.PendingRegistration,
                Types = [AccountType.EarlyCareerSocialWorker],
                Id = 2
            },
            new Account
            {
                FirstName = "Ricardo",
                LastName = "Athanasopoulos",
                Status = AccountStatus.Active,
                Types = [AccountType.Assessor, AccountType.Coordinator],
                Id = 3
            },
            new Account
            {
                FirstName = "Sheena",
                LastName = "Newman",
                Status = AccountStatus.Active,
                Types = [AccountType.EarlyCareerSocialWorker],
                Id = 4
            },
            new Account
            {
                FirstName = "Yavuz",
                LastName = "Karci",
                Status = AccountStatus.Active,
                Types = [AccountType.EarlyCareerSocialWorker],
                Id = 5
            }
        ]);
    }
}
