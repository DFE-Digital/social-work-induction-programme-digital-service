namespace Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;

public interface IMoodleServiceClient
{
    public IUserOperations User { get; init; }
    public ICourseOperations Course { get; init; }
}
