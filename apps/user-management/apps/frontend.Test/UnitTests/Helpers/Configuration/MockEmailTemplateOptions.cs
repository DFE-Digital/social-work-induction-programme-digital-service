using Dfe.Sww.Ecf.Frontend.Configuration.Notification;
using Dfe.Sww.Ecf.Frontend.Models;
using Microsoft.Extensions.Options;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Configuration;

public class MockEmailTemplateOptions : Mock<IOptions<EmailTemplateOptions>>
{
    public readonly EmailTemplateOptions EmailTemplateOptions =
        new()
        {
            Roles = Enum.GetNames<AccountType>()
                .ToDictionary(
                    accountType => accountType,
                    _ => new RoleEmailTemplateConfiguration
                    {
                        Invitation = Guid.NewGuid(),
                        Welcome = Guid.NewGuid(),
                        Pause = Guid.NewGuid(),
                        Unpause = Guid.NewGuid(),
                        Link = Guid.NewGuid(),
                        Unlink = Guid.NewGuid()
                    }
                )
        };

    public MockEmailTemplateOptions()
    {
        Setup(x => x.Value).Returns(EmailTemplateOptions);
    }
}
