
namespace Dfe.Sww.Ecf.Core.Services.NameSynonyms;

public interface INameSynonymProvider
{
    Task<IReadOnlyDictionary<string, IReadOnlyCollection<string>>> GetAllNameSynonyms();
}
