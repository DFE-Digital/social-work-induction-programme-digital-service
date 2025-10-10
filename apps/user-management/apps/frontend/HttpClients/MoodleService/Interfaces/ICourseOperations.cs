using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Courses;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;

public interface ICourseOperations
{
    Task<EnrolUserResponse> EnrolUserAsync(EnrolUserRequest request);
}
