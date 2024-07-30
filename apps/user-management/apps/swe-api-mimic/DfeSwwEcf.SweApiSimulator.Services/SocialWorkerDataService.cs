using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using CsvHelper;
using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Models.Config;
using DfeSwwEcf.SweApiSimulator.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace DfeSwwEcf.SweApiSimulator.Services;

/// <inheritdoc />
[ExcludeFromCodeCoverage]
public class SocialWorkerDataService : ISocialWorkerDataService
{
    private readonly IEnumerable<SocialWorker> _socialWorkers;

    public SocialWorkerDataService(IOptions<CsvFileOptions> csvConfig)
    {
        if (string.IsNullOrWhiteSpace(csvConfig.Value.FileLocation))
        {
            throw new NullReferenceException();
        }

        using var reader = new StreamReader(csvConfig.Value.FileLocation);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        _socialWorkers = csv.GetRecords<SocialWorker>().ToList();
    }

    /// <inheritdoc />
    public SocialWorker? GetById(string? swId)
    {
        var socialWorker = _socialWorkers.FirstOrDefault(x => x.Id == swId);

        return socialWorker;
    }
}
