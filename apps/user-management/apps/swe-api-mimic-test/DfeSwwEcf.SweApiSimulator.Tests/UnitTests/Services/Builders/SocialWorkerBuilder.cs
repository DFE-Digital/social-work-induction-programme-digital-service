using DfeSwwEcf.SweApiSimulator.Models;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Services.Builders;

public class SocialWorkerBuilder
{
    private string? _registrationNumber;

    public static SocialWorkerBuilder CreateNew()
    {
        return new SocialWorkerBuilder();
    }

    public SocialWorkerBuilder AddRegistrationNumber(string? registrationNumber)
    {
        _registrationNumber = registrationNumber;

        return this;
    }

    public SocialWorker Build()
    {
        return new SocialWorker { RegistrationNumber = _registrationNumber };
    }
}
