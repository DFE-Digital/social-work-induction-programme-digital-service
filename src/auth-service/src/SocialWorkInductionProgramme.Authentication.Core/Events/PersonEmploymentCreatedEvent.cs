using SocialWorkInductionProgramme.Authentication.Core.Events.Models;

namespace SocialWorkInductionProgramme.Authentication.Core.Events;

public record PersonEmploymentCreatedEvent : EventBase, IEventWithPersonId
{
    public required Guid PersonId { get; init; }
    public required PersonEmployment PersonEmployment { get; init; }
}
