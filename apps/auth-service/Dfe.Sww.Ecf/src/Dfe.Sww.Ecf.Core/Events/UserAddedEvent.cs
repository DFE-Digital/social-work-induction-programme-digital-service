using Dfe.Sww.Ecf.Core.Events.Models;

namespace Dfe.Sww.Ecf.Core.Events;

public record UserAddedEvent : EventBase
{
    public required User User { get; init; }
}
