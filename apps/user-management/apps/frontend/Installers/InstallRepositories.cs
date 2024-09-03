using System.Diagnostics.CodeAnalysis;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Repositories;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Installers;

/// <summary>
/// Install Repository Dependencies
/// </summary>
[ExcludeFromCodeCoverage]
public static class InstallRepositories
{
    /// <summary>
    /// Add Repository Dependencies
    /// </summary>
    /// <param name="services"></param>
    public static void AddRepository(this IServiceCollection services)
    {
        var accountsRepository = new InMemoryAccountRepository();

        // TODO: Remove this once the DAL is in place
        accountsRepository.AddRange(
            [
                new Account
                {
                    FirstName = "Ellen",
                    LastName = "Ripley",
                    Email = "ellen@rbcouncil.org.uk",
                    SocialWorkEnglandNumber = "SW798",
                    Status = AccountStatus.Paused,
                    Types = [AccountType.EarlyCareerSocialWorker],
                    Id = Guid.NewGuid()
                },
                new Account
                {
                    FirstName = "Joe",
                    LastName = "Bloggs",
                    Email = "joe@rbcouncil.org.uk",
                    Status = AccountStatus.Active,
                    Types = [AccountType.Coordinator],
                    Id = Guid.NewGuid()
                },
                new Account
                {
                    FirstName = "Laura",
                    LastName = "Barn",
                    Email = "laura@rbcouncil.org.uk",
                    Status = AccountStatus.PendingRegistration,
                    Types = [AccountType.EarlyCareerSocialWorker],
                    Id = Guid.NewGuid()
                },
                new Account
                {
                    FirstName = "Ricardo",
                    LastName = "Athanasopoulos",
                    Email = "ricardo@rbcouncil.org.uk",
                    Status = AccountStatus.Active,
                    Types = [AccountType.Assessor, AccountType.Coordinator],
                    Id = Guid.NewGuid()
                },
                new Account
                {
                    FirstName = "Sheena",
                    LastName = "Newman",
                    Email = "sheena@rbcouncil.org.uk",
                    SocialWorkEnglandNumber = "SW2478",
                    Status = AccountStatus.Inactive,
                    Types = [AccountType.EarlyCareerSocialWorker],
                    Id = Guid.NewGuid()
                },
                new Account
                {
                    FirstName = "Yavuz",
                    LastName = "Karci",
                    Email = "Yavuz@rbcouncil.org.uk",
                    SocialWorkEnglandNumber = "SW142",
                    Status = AccountStatus.Active,
                    Types = [AccountType.EarlyCareerSocialWorker],
                    Id = Guid.NewGuid()
                }
            ]
        );

        services.AddSingleton<IAccountRepository>(accountsRepository);
    }
}
