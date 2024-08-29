using System.Net;
using DfeSwwEcf.SweApiSimulator.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Controllers.SocialWorkerTests;

public class GetByIdShould : SocialWorkerTestsTestBase
{
    [Fact]
    public void WhenCalled_ReturnSocialWorker()
    {
        // Arrange
        var socialWorkerId = 1;

        var expectedResponse = new SocialWorkerResponse
        {
            SocialWorker = new()
            {
                RegistrationNumber = $"SW{socialWorkerId}",
                RegisteredName = "Ralph Cormier",
                Status = "Registered",
                TownOfEmployment = "Workington",
                RegisteredFrom = new DateTime(2012, 8, 1),
                RegisteredUntil = new DateTime(2024, 11, 30),
                Registered = "True"
            }
        };

        MockSocialWorkerService
            .Setup(x => x.GetById(socialWorkerId.ToString()))
            .Returns(expectedResponse);

        // Act
        var response = Sut.GetById(socialWorkerId.ToString());

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<OkObjectResult>();
        var result = response as OkObjectResult;

        result!.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<SocialWorker>();
        result.Value.Should().BeEquivalentTo(expectedResponse.SocialWorker);

        MockSocialWorkerService.Verify(x => x.GetById(socialWorkerId.ToString()), Times.Once);
        VerifyAllNoOtherCall();
    }

    [Theory]
    [MemberData(nameof(ErrorTestData))]
    public void WhenErrorIsReturned_ReturnsRelevantResponse(
        HttpStatusCode statusCode,
        string errorMessage,
        object? expectedApiResponse,
        Type responseType
    )
    {
        // Arrange
        var socialWorkerId = 1;

        var socialWorkerResponse = new SocialWorkerResponse
        {
            ErrorDetails = new() { HttpStatusCode = statusCode, ErrorMessage = errorMessage }
        };

        MockSocialWorkerService
            .Setup(x => x.GetById(socialWorkerId.ToString()))
            .Returns(socialWorkerResponse);

        // Act
        var response = Sut.GetById(socialWorkerId.ToString());

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<ObjectResult>();
        var result = response as ObjectResult;

        result!.StatusCode.Should().Be((int)statusCode);

        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType(responseType);
        result.Value.Should().BeEquivalentTo(expectedApiResponse);

        MockSocialWorkerService.Verify(x => x.GetById(socialWorkerId.ToString()), Times.Once);
        VerifyAllNoOtherCall();
    }

    public static IEnumerable<object[]> ErrorTestData =>
        new List<object[]>
        {
            new object[]
            {
                HttpStatusCode.OK,
                "Invalid Request",
                "Invalid Request",
                typeof(string)
            },
            new object[]
            {
                HttpStatusCode.UnprocessableEntity,
                "Please enter valid integer value",
                new NonIntSweIdResponse { Error = "Please enter valid integer value", },
                typeof(NonIntSweIdResponse)
            },
        };
}
