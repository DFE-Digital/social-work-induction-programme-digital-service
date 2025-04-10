namespace SocialWorkInductionProgramme.Authentication.Core.Events;

public interface IEventWithKey
{
    string? Key { get; }
}
