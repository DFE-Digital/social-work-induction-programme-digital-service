using System.Security.Claims;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Mappers;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services;

public class UserService(
    IAuthServiceClient authServiceClient,
    IModelMapper<Person, User> mapper
) : IUserService
{
    public async Task<PaginationResult<User>> GetAllAsync(PaginationRequest request)
    {
        var persons = await authServiceClient.Users.GetAllAsync(request);

        var users = new PaginationResult<User>
        {
            Records = persons.Records.Select(mapper.MapToBo).ToList(),
            MetaData = persons.MetaData
        };

        return users;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var person = await authServiceClient.Users.GetByIdAsync(id);

        if (person is null)
        {
            return null;
        }

        return mapper.MapToBo(person);
    }

    public async Task<User> CreateAsync(User user)
    {
        if (
            string.IsNullOrWhiteSpace(user.FirstName)
            || string.IsNullOrWhiteSpace(user.LastName)
            || string.IsNullOrWhiteSpace(user.Email)
        )
        {
            throw new ArgumentException("First name, last name, and email are required");
        }

        var organisationId = authServiceClient.HttpContextService.GetOrganisationId();

        var person = await authServiceClient.Users.CreateAsync(
            new CreatePersonRequest
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.Email,
                SocialWorkEnglandNumber = user.SocialWorkEnglandNumber,
                Roles = user.Types ?? [],
                Status = user.Status,
                OrganisationId = new Guid(organisationId),
                ExternalUserId = user.ExternalUserId,
                IsFunded = user.IsFunded
            }
        );

        return mapper.MapToBo(person);
    }

    public async Task<User> UpdateAsync(User updatedUser)
    {
        if (
            string.IsNullOrWhiteSpace(updatedUser.FirstName)
            || string.IsNullOrWhiteSpace(updatedUser.LastName)
            || string.IsNullOrWhiteSpace(updatedUser.Email)
        )
        {
            throw new ArgumentException("Person ID, First name, last name, and email are required");
        }

        var person = await authServiceClient.Users.UpdateAsync(
            new UpdatePersonRequest
            {
                PersonId = updatedUser.Id,
                FirstName = updatedUser.FirstName,
                LastName = updatedUser.LastName,
                EmailAddress = updatedUser.Email,
                SocialWorkEnglandNumber = updatedUser.SocialWorkEnglandNumber,
                Roles = updatedUser.Types ?? [],
                Status = updatedUser.Status
            }
        );

        return mapper.MapToBo(person);
    }
}
