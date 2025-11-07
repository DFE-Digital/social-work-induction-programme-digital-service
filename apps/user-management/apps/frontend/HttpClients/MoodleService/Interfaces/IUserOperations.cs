using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Users;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;

public interface IUserOperations
{
    Task<MoodleUserResponse> CreateAsync(MoodleUserRequest request);
    Task<MoodleUserResponse> UpdateAsync(MoodleUserRequest request);
}
