namespace SocialWorkInductionProgramme.Frontend.HttpClients.SocialWorkEngland.Interfaces;

public interface ISocialWorkEnglandClient
{
    public ISocialWorkersOperations SocialWorkers { get; init; }
}
