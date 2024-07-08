using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Services.NameSynonyms;

namespace Dfe.Sww.Ecf.Core.Jobs;

public class PopulateNameSynonymsJob(EcfDbContext dbContext, INameSynonymProvider nameSynonymProvider)
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        var namesLookup = await nameSynonymProvider.GetAllNameSynonyms();

        foreach (var (name, synonyms) in namesLookup)
        {
            var nameSynonyms = await dbContext.NameSynonyms
                .Where(ns => ns.Name == name)
                .SingleOrDefaultAsync(cancellationToken);

            if (nameSynonyms == null)
            {
                nameSynonyms = new NameSynonyms
                {
                    Name = name,
                    Synonyms = synonyms.ToArray(),
                };

                dbContext.NameSynonyms.Add(nameSynonyms);
            }
            else
            {
                nameSynonyms.Synonyms = synonyms.ToArray();
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
