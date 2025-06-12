using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

public interface IRegisterSocialWorkerJourneyService
{
    Task<DateOnly?> GetDateOfBirthAsync(Guid accountId);
    Task SetDateOfBirthAsync(Guid accountId, DateOnly? dateOfBirth);
    void ResetRegisterSocialWorkerJourneyModel(Guid accountId);
    Task<Account> CompleteJourneyAsync(Guid accountId);
}
