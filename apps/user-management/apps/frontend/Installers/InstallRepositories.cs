using System.Diagnostics.CodeAnalysis;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Repositories;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Installers;

/// <summary>
/// Install Repository Dependencies
/// </summary>
[ExcludeFromCodeCoverage]
public static class InstallServices
{
    /// <summary>
    /// Add Repository Dependencies
    /// </summary>
    /// <param name="services"></param>
    public static void AddRepository(this IServiceCollection services)
    {
        var accountsRepository = new AccountRepository();

        // TODO: Remove this once the DAL is in place
        accountsRepository.AddRange(
            [
                new Account
                {
                    FirstName = "Ellen",
                    LastName = "Ripley",
                    Status = AccountStatus.Paused,
                    Types = [AccountType.EarlyCareerSocialWorker],
                    Id = Guid.NewGuid()
                },
                new Account
                {
                    FirstName = "Joe",
                    LastName = "Bloggs",
                    Status = AccountStatus.Active,
                    Types = [AccountType.Coordinator],
                    Id = Guid.NewGuid()
                },
                new Account
                {
                    FirstName = "Laura",
                    LastName = "Barn",
                    Status = AccountStatus.PendingRegistration,
                    Types = [AccountType.EarlyCareerSocialWorker],
                    Id = Guid.NewGuid()
                },
                new Account
                {
                    FirstName = "Ricardo",
                    LastName = "Athanasopoulos",
                    Status = AccountStatus.Active,
                    Types = [AccountType.Assessor, AccountType.Coordinator],
                    Id = Guid.NewGuid()
                },
                new Account
                {
                    FirstName = "Sheena",
                    LastName = "Newman",
                    Status = AccountStatus.Active,
                    Types = [AccountType.EarlyCareerSocialWorker],
                    Id = Guid.NewGuid()
                },
                new Account
                {
                    FirstName = "Yavuz",
                    LastName = "Karci",
                    Status = AccountStatus.Active,
                    Types = [AccountType.EarlyCareerSocialWorker],
                    Id = Guid.NewGuid()
                }
            ]
        );

        services.AddSingleton<IAccountRepository>(accountsRepository);
    }
}
