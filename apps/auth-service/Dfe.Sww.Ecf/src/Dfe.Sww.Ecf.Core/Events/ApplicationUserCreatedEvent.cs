using Dfe.Sww.Ecf.Core.Events.Models;

namespace Dfe.Sww.Ecf.Core.Events;

public record class ApplicationUserCreatedEvent : EventBase
{
    public required ApplicationUser ApplicationUser { get; init; }
}
