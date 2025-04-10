namespace SocialWorkInductionProgramme.Authentication.Core.Events.Processing;

public interface IEventObserver
{
    Task OnEventSaved(EventBase @event);
}
