
namespace SocialWorkInductionProgramme.Authentication.Core.Services.NameSynonyms;

public interface INameSynonymProvider
{
    Task<IReadOnlyDictionary<string, IReadOnlyCollection<string>>> GetAllNameSynonyms();
}
