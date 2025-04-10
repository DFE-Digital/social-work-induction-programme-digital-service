using SocialWorkInductionProgramme.Frontend.Configuration.Notification;
using SocialWorkInductionProgramme.Frontend.Models;
using Microsoft.Extensions.Options;
using Moq;

namespace SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers.Configuration;

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
                        Welcome = Guid.NewGuid()
                    }
                )
        };

    public MockEmailTemplateOptions()
    {
        Setup(x => x.Value).Returns(EmailTemplateOptions);
    }
}
