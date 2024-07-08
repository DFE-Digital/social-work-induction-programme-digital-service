namespace Dfe.Sww.Ecf.Core.Events.Processing;

public class NoopEventObserver : IEventObserver
{
    public Task OnEventSaved(EventBase @event) => Task.CompletedTask;
}
