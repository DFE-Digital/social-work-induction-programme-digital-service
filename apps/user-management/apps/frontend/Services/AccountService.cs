using System.Security.Claims;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Mappers;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services;

public class AccountService(
    IAuthServiceClient authServiceClient,
    IModelMapper<Person, Account> mapper
) : IAccountService
{
    public async Task<PaginationResult<Account>> GetAllAsync(PaginationRequest request)
    {
        var persons = await authServiceClient.Accounts.GetAllAsync(request);

        var accounts = new PaginationResult<Account>
        {
            Records = persons.Records.Select(mapper.MapToBo).ToList(),
            MetaData = persons.MetaData
        };

        return accounts;
    }

    public async Task<Account?> GetByIdAsync(Guid id)
    {
        var person = await authServiceClient.Accounts.GetByIdAsync(id);

        if (person is null)
        {
            return null;
        }

        return mapper.MapToBo(person);
    }

    public async Task<Account> CreateAsync(Account account)
    {
        if (
            string.IsNullOrWhiteSpace(account.FirstName)
            || string.IsNullOrWhiteSpace(account.LastName)
            || string.IsNullOrWhiteSpace(account.Email)
        )
        {
            throw new ArgumentException("First name, last name, and email are required");
        }

        var organisationId = authServiceClient.HttpContextService.GetOrganisationId();

        var person = await authServiceClient.Accounts.CreateAsync(
            new CreatePersonRequest
            {
                FirstName = account.FirstName,
                LastName = account.LastName,
                MiddleName = account.MiddleNames,
                EmailAddress = account.Email,
                SocialWorkEnglandNumber = account.SocialWorkEnglandNumber,
                Roles = account.Types ?? [],
                Status = account.Status,
                OrganisationId = new Guid(organisationId),
                ExternalUserId = account.ExternalUserId,
                IsFunded = account.IsFunded,
                ProgrammeStartDate = account.ProgrammeStartDate,
                ProgrammeEndDate = account.ProgrammeEndDate
            }
        );

        return mapper.MapToBo(person);
    }

    public async Task<Account> UpdateAsync(Account updatedAccount)
    {
        if (
            string.IsNullOrWhiteSpace(updatedAccount.FirstName)
            || string.IsNullOrWhiteSpace(updatedAccount.LastName)
            || string.IsNullOrWhiteSpace(updatedAccount.Email)
        )
        {
            throw new ArgumentException("Person ID, First name, last name, and email are required");
        }

        var person = await authServiceClient.Accounts.UpdateAsync(
            new UpdatePersonRequest
            {
                PersonId = updatedAccount.Id,
                FirstName = updatedAccount.FirstName,
                LastName = updatedAccount.LastName,
                EmailAddress = updatedAccount.Email,
                SocialWorkEnglandNumber = updatedAccount.SocialWorkEnglandNumber,
                Roles = updatedAccount.Types ?? [],
                Status = updatedAccount.Status,
                DateOfBirth = updatedAccount.DateOfBirth,
                UserSex = updatedAccount.UserSex,
                GenderMatchesSexAtBirth = updatedAccount.GenderMatchesSexAtBirth,
                OtherGenderIdentity = updatedAccount.OtherGenderIdentity,
                EthnicGroup = updatedAccount.EthnicGroup,
                EthnicGroupWhite = updatedAccount.EthnicGroupWhite,
                OtherEthnicGroupWhite = updatedAccount.OtherEthnicGroupWhite,
                EthnicGroupMixed = updatedAccount.EthnicGroupMixed,
                OtherEthnicGroupMixed = updatedAccount.OtherEthnicGroupMixed,
                EthnicGroupBlack = updatedAccount.EthnicGroupBlack,
                OtherEthnicGroupBlack = updatedAccount.OtherEthnicGroupBlack,
                EthnicGroupOther = updatedAccount.EthnicGroupOther,
                OtherEthnicGroupOther = updatedAccount.OtherEthnicGroupOther,
                Disability = updatedAccount.Disability,
                SocialWorkEnglandRegistrationDate = updatedAccount.SocialWorkEnglandRegistrationDate,
                HighestQualification = updatedAccount.HighestQualification,
                RouteIntoSocialWork = updatedAccount.RouteIntoSocialWork,
                OtherRouteIntoSocialWork = updatedAccount.OtherRouteIntoSocialWork,
                SocialWorkQualificationEndYear = updatedAccount.SocialWorkQualificationEndYear
            }
        );

        return mapper.MapToBo(person);
    }
}
