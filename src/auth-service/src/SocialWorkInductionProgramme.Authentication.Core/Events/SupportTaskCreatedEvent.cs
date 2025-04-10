using SocialWorkInductionProgramme.Authentication.Core.Events.Models;

namespace SocialWorkInductionProgramme.Authentication.Core.Events;

public record SupportTaskCreatedEvent : EventBase
{
    public required SupportTask SupportTask { get; init; }
}
