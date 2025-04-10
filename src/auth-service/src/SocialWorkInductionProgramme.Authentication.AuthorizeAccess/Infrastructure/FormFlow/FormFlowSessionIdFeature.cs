namespace SocialWorkInductionProgramme.Authentication.AuthorizeAccess.Infrastructure.FormFlow;

public class FormFlowSessionIdFeature(string sessionId)
{
    public string SessionId { get; } = sessionId;
}
