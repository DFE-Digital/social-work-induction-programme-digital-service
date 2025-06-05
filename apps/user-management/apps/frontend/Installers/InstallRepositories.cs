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
                new User
                {
                    FirstName = "Ellen",
                    LastName = "Ripley",
                    Email = "ellen@rbcouncil.org.uk",
                    SocialWorkEnglandNumber = "SW798",
                    Status = UserStatus.Active,
                    Types = [UserType.EarlyCareerSocialWorker],
                    Id = Guid.NewGuid()
                },
                new User
                {
                    FirstName = "Joe",
                    LastName = "Bloggs",
                    Email = "joe@rbcouncil.org.uk",
                    Status = UserStatus.Active,
                    Types = [UserType.Coordinator],
                    Id = Guid.NewGuid()
                },
                new User
                {
                    FirstName = "Laura",
                    LastName = "Barn",
                    Email = "laura@rbcouncil.org.uk",
                    Status = UserStatus.PendingRegistration,
                    Types = [UserType.EarlyCareerSocialWorker],
                    Id = Guid.NewGuid()
                },
                new User
                {
                    FirstName = "Ricardo",
                    LastName = "Athanasopoulos",
                    Email = "ricardo@rbcouncil.org.uk",
                    Status = UserStatus.Active,
                    Types = [UserType.Assessor, UserType.Coordinator],
                    Id = Guid.NewGuid()
                },
                new User
                {
                    FirstName = "Sheena",
                    LastName = "Newman",
                    Email = "sheena@rbcouncil.org.uk",
                    SocialWorkEnglandNumber = "SW2478",
                    Status = UserStatus.Active,
                    Types = [UserType.EarlyCareerSocialWorker],
                    Id = Guid.NewGuid()
                },
                new User
                {
                    FirstName = "Yavuz",
                    LastName = "Karci",
                    Email = "Yavuz@rbcouncil.org.uk",
                    SocialWorkEnglandNumber = "SW142",
                    Status = UserStatus.Active,
                    Types = [UserType.EarlyCareerSocialWorker],
                    Id = Guid.NewGuid()
                }
            ]
        );

        services.AddSingleton<IAccountRepository>(accountsRepository);
    }
}
