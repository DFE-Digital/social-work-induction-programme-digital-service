using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator;
using DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator.Interfaces;
using DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Helpers;
using DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Services.Builders;
using FluentAssertions;
using NonIntSweIdResponse = DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator.NonIntSweIdResponse;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Services.ResponsesGenerator;

public class SocialWorkerResponseFactoryShould
{
    private readonly ISocialWorkerResponseFactory _sut = new SocialWorkerResponseFactory();

    [Fact]
    public void WhenSocialWorkerRecordNotFound_ReturnsNotFoundResponse()
    {
        // Act
        var response = _sut.Create("1", null);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<NotFoundResponse>();
    }

    [Theory]
    [MemberData(nameof(SocialWorkerResponseTestData))]
    public void WhenCalled_ReturnsCorrectResponseType(
        SocialWorker? socialWorker,
        Type expectedResponse
    )
    {
        // Act
        var response = _sut.Create(socialWorker?.RegistrationNumber, socialWorker);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType(expectedResponse);
    }

    public static IEnumerable<object?[]> SocialWorkerResponseTestData =>
        new List<object?[]>
        {
            new object?[]
            {
                new SocialWorkerBuilder().AddRegistrationNumber(null).Build(),
                typeof(SweIdNullErrorResponse)
            },
            new object?[]
            {
                new SocialWorkerBuilder().AddRegistrationNumber("SW1234").Build(),
                typeof(NonIntSweIdResponse)
            },
            new object?[]
            {
                new SocialWorkerBuilder().AddRegistrationNumber("01").Build(),
                typeof(NonIntSweIdResponse)
            },
            new object?[]
            {
                new SocialWorkerBuilder().AddRegistrationNumber("-1").Build(),
                typeof(NonIntSweIdResponse)
            },
            new object?[]
            {
                new SocialWorkerBuilder().AddRegistrationNumber("99111222333").Build(),
                typeof(SweIdMaxIntErrorResponse)
            },
            new object?[]
            {
                new SocialWorkerBuilder().AddRegistrationNumber("4296").Build(),
                typeof(InvalidErrorResponse)
            },
            new object?[]
            {
                new SocialWorkerBuilder().AddRegistrationNumber("5604").Build(),
                typeof(ValidResponse)
            }
        };
}
