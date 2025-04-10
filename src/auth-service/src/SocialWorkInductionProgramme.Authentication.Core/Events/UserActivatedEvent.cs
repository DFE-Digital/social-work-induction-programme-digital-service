using SocialWorkInductionProgramme.Authentication.Core.Events.Models;

namespace SocialWorkInductionProgramme.Authentication.Core.Events;

public record UserActivatedEvent : EventBase
{
    public required User User { get; init; }
}
