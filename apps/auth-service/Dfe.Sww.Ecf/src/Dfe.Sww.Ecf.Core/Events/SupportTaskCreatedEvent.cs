using Dfe.Sww.Ecf.Core.Events.Models;

namespace Dfe.Sww.Ecf.Core.Events;

public record SupportTaskCreatedEvent : EventBase
{
    public required SupportTask SupportTask { get; init; }
}
