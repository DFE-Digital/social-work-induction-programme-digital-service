using Bogus;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;

public sealed class NotificationRequestFaker : Faker<NotificationRequest>
{
    public NotificationRequestFaker()
    {
        RuleFor(a => a.EmailAddress, f => f.Internet.Email());
        RuleFor(a => a.TemplateId, f => f.Random.Guid());
        RuleFor(a => a.Reference, f => f.Random.String());
        RuleFor(a => a.EmailReplyToId, f => f.Random.Guid());
    }
}
