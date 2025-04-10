using SocialWorkInductionProgramme.Authentication.Core.Events.Models;

namespace SocialWorkInductionProgramme.Authentication.Core.Events;

public record UserAddedEvent : EventBase
{
    public required User User { get; init; }
}
