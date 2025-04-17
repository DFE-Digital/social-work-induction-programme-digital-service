using Dfe.Sww.Ecf.Core.Events.Models;

namespace Dfe.Sww.Ecf.Core.Events;

public record PersonEmploymentCreatedEvent : EventBase, IEventWithPersonId
{
    public required Guid PersonId { get; init; }
    public required PersonEmployment PersonEmployment { get; init; }
}
