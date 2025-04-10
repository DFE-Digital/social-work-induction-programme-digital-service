namespace SocialWorkInductionProgramme.Authentication.Core.Events.Processing;

public class NoopEventObserver : IEventObserver
{
    public Task OnEventSaved(EventBase @event) => Task.CompletedTask;
}
