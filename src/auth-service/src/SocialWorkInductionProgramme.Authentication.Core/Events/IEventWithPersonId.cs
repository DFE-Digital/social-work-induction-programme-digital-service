namespace SocialWorkInductionProgramme.Authentication.Core.Events;

public interface IEventWithPersonId
{
    Guid PersonId { get; }
}
