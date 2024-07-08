using Dfe.Sww.Ecf.Core.Events.Models;

namespace Dfe.Sww.Ecf.Core.Events;

public record ApiKeyCreatedEvent : EventBase, IEventWithApiKey
{
    public required ApiKey ApiKey { get; init; }
}
