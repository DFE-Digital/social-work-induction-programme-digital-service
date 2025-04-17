namespace Dfe.Sww.Ecf.Core.Events.Processing;

public interface IEventObserver
{
    Task OnEventSaved(EventBase @event);
}
