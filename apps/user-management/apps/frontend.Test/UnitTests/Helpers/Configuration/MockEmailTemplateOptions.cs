using Bogus;
using Dfe.Sww.Ecf.Frontend.Configuration.Notification;
using Dfe.Sww.Ecf.Frontend.Models;
using Microsoft.Extensions.Options;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Configuration;

public class MockEmailTemplateOptions : Mock<IOptions<EmailTemplateOptions>>
{
    private readonly Faker _faker = new();

    public MockEmailTemplateOptions()
    {
        Options =
            new EmailTemplateOptions
            {
                PrimaryCoordinatorInvitationEmail = _faker.Random.Guid(),
                Invitation = _faker.Random.Guid()
            };
        Setup(x => x.Value).Returns(Options);
    }

    public EmailTemplateOptions Options { get; }
}
