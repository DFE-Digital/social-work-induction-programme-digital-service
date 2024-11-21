using Dfe.Sww.Ecf.Frontend.Configuration.Notification;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Mappers;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Repositories;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Services;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public abstract class ManageAccountsPageTestBase<[MeansTestSubject] T> : PageModelTestBase<T>
    where T : PageModel
{
    private protected Mock<IAccountService> MockAccountService { get; }

    private protected InMemoryAccountRepository AccountRepository { get; }
    private protected AccountFaker AccountFaker { get; }

    private protected CreateAccountJourneyService CreateAccountJourneyService { get; }

    private protected EditAccountJourneyService EditAccountJourneyService { get; }

    private protected MockAuthServiceClient MockAuthServiceClient { get; }

    private protected Mock<ISocialWorkEnglandService> MockSocialWorkEnglandService { get; }

    private protected Mock<INotificationServiceClient> MockNotificationServiceClient { get; }

    private protected Mock<INotificationOperations> MockNotificationOperations { get; }

    private protected Mock<IOptions<EmailTemplateOptions>> MockEmailTemplateOptions { get; }

    protected ManageAccountsPageTestBase()
    {
        AccountFaker = new AccountFaker();
        MockAccountService = new Mock<IAccountService>();
        AccountRepository = new InMemoryAccountRepository();
        AccountRepository.AddRange(AccountFaker.Generate(10));

        MockEmailTemplateOptions = new Mock<IOptions<EmailTemplateOptions>>();
        MockEmailTemplateOptions
            .Setup(x => x.Value)
            .Returns(
                new EmailTemplateOptions
                {
                    Roles = new Dictionary<string, RoleEmailTemplateConfiguration>
                    {
                        {
                            AccountType.Assessor.ToString(),
                            new RoleEmailTemplateConfiguration
                            {
                                Invitation = Guid.NewGuid(),
                                Welcome = Guid.NewGuid()
                            }
                        },
                        {
                            AccountType.EarlyCareerSocialWorker.ToString(),
                            new RoleEmailTemplateConfiguration
                            {
                                Invitation = Guid.NewGuid(),
                                Welcome = Guid.NewGuid()
                            }
                        },
                        {
                            AccountType.Coordinator.ToString(),
                            new RoleEmailTemplateConfiguration
                            {
                                Invitation = Guid.NewGuid(),
                                Welcome = Guid.NewGuid()
                            }
                        }
                    }
                }
            );

        MockAuthServiceClient = new MockAuthServiceClient();
        MockSocialWorkEnglandService = new Mock<ISocialWorkEnglandService>();
        MockNotificationServiceClient = new Mock<INotificationServiceClient>();
        MockNotificationOperations = new Mock<INotificationOperations>();
        MockNotificationServiceClient
            .SetupGet(client => client.Notification)
            .Returns(MockNotificationOperations.Object);

        var httpContextAccessor = new HttpContextAccessor { HttpContext = HttpContext };
        CreateAccountJourneyService = new CreateAccountJourneyService(
            httpContextAccessor,
            MockNotificationServiceClient.Object,
            MockEmailTemplateOptions.Object,
            MockAuthServiceClient.Object,
            new AccountService(MockAuthServiceClient.Object, new AccountMapper()),
            new FakeLinkGenerator()
        );
        EditAccountJourneyService = new EditAccountJourneyService(
            httpContextAccessor,
            AccountRepository
        );
    }
}
