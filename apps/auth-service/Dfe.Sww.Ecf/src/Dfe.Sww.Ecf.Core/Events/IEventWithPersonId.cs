namespace Dfe.Sww.Ecf.Core.Events;

public interface IEventWithPersonId
{
    Guid PersonId { get; }
}
