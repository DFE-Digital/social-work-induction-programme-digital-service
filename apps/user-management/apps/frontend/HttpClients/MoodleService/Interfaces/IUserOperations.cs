using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Users;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;

public interface IUserOperations
{
    Task<MoodleUserResponse> CreateUserAsync(MoodleUserRequest request);
    Task<MoodleUserResponse> UpdateUserAsync(MoodleUserRequest request);
}
