namespace Dfe.Sww.Ecf.Core.Services.TrnGenerationApi;

public interface ITrnGenerationApiClient
{
    Task<string> GenerateTrn();
}
