namespace SocialWorkInductionProgramme.Authentication.Core.Services.TrnGenerationApi;

public interface ITrnGenerationApiClient
{
    Task<string> GenerateTrn();
}
