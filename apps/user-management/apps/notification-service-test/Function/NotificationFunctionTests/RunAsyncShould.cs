using System.Net;
using DfeSwwEcf.NotificationService.Models;
using DfeSwwEcf.NotificationService.Services.Interfaces;
using DfeSwwEcf.NotificationService.UnitTests.Helpers;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using System.Text.Json;

namespace DfeSwwEcf.NotificationService.UnitTests.Function.NotificationFunctionTests;

public class RunAsyncShould
{
    private readonly Mock<ILogger<NotificationFunction>> _mockLog;
    private readonly Mock<IValidator<NotificationRequest>> _mockValidator;
    private readonly Mock<INotificationCommand> _mockNotificationCommand;
    private readonly NotificationFunction _sut;

    public RunAsyncShould()
    {
        _mockLog = new();
        _mockValidator = new();
        _mockNotificationCommand = new();

        _sut = new(_mockLog.Object, _mockValidator.Object, _mockNotificationCommand.Object);
    }

    [Fact]
    public async Task WhenCalled_SendsEmailNotification()
    {
        // Arrange
        var notificationRequest = new NotificationRequest
        {
            EmailAddress = "test@test.com",
            TemplateId = Guid.NewGuid(),
            Reference = "string",
            EmailReplyToId = Guid.NewGuid(),
            Personalisation = new Dictionary<string, string>()
            {
                { "first_name", "Amala" }
            }
        };

        var expectedResponse = new NotificationResponse
        {
            StatusCode = HttpStatusCode.OK
        };

        var json = JsonSerializer.Serialize(notificationRequest);
        var request = CreateHttpRequest(json);

        _mockValidator
            .Setup(x => x.ValidateAsync(MoqHelpers.ShouldBeEquivalentTo(notificationRequest), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockNotificationCommand
            .Setup(x => x.SendNotificationAsync(MoqHelpers.ShouldBeEquivalentTo(notificationRequest)))
            .ReturnsAsync(expectedResponse);

        // Act
        var response = await _sut.RunAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<StatusCodeResult>();

        _mockValidator.Verify(x => x.ValidateAsync(MoqHelpers.ShouldBeEquivalentTo(notificationRequest), It.IsAny<CancellationToken>()), Times.Once);
        _mockNotificationCommand.Verify(x => x.SendNotificationAsync(MoqHelpers.ShouldBeEquivalentTo(notificationRequest)), Times.Once);

        _mockValidator.VerifyNoOtherCalls();
        _mockNotificationCommand.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task WhenValidationFails_ReturnsBadRequestWithErrors()
    {
        // Arrange
        var notificationRequest = new NotificationRequest
        {
            EmailAddress = "NOT AN EMAIL",
        };

        var json = JsonSerializer.Serialize(notificationRequest);
        var request = CreateHttpRequest(json);

        var validationResult = new ValidationResult
        {
            Errors = new List<ValidationFailure>
            {
                new()
                {
                    ErrorMessage = "EMAIL NOT AN EMAIL",
                    AttemptedValue = "NOT AN EMAIL",
                    PropertyName = nameof(NotificationRequest.EmailAddress),
                    Severity = Severity.Error,
                }

            }
        };

        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<NotificationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var response = await _sut.RunAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestObjectResult>();

        _mockValidator.Verify(x => x.ValidateAsync(MoqHelpers.ShouldBeEquivalentTo(notificationRequest), It.IsAny<CancellationToken>()), Times.Once);
        _mockNotificationCommand.Verify(x => x.SendNotificationAsync(It.IsAny<NotificationRequest>()), Times.Never);

        _mockValidator.VerifyNoOtherCalls();
        _mockNotificationCommand.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task WhenNotJson_ReturnsBadRequestWithMessage()
    {
        // Arrange
        var body = "not valid json";

        var request = CreateHttpRequest(body);

        var responseMessage = "Request body is not valid json";

        // Act
        var response = await _sut.RunAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestObjectResult>();

        var responseObject = response as BadRequestObjectResult;
        responseObject!.Value.Should().NotBeNull();
        responseObject.Value.Should().BeEquivalentTo(responseMessage);

        _mockValidator.Verify(x => x.Validate(It.IsAny<NotificationRequest>()), Times.Never);
        _mockNotificationCommand.Verify(x => x.SendNotificationAsync(It.IsAny<NotificationRequest>()), Times.Never);

        _mockValidator.VerifyNoOtherCalls();
        _mockNotificationCommand.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task WhenNull_ReturnsBadRequestWithMessage()
    {
        // Arrange
        var body = "null";

        var request = CreateHttpRequest(body);

        var responseMessage = "Request body failed to deserialise";

        // Act
        var response = await _sut.RunAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestObjectResult>();

        var responseObject = response as BadRequestObjectResult;
        responseObject!.Value.Should().NotBeNull();
        responseObject.Value.Should().BeEquivalentTo(responseMessage);

        _mockValidator.Verify(x => x.Validate(It.IsAny<NotificationRequest>()), Times.Never);
        _mockNotificationCommand.Verify(x => x.SendNotificationAsync(It.IsAny<NotificationRequest>()), Times.Never);

        _mockValidator.VerifyNoOtherCalls();
        _mockNotificationCommand.VerifyNoOtherCalls();
    }

    private HttpRequest CreateHttpRequest(string body)
    {
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(body));
        var context = new DefaultHttpContext();
        var request = context.Request;
        request.Body = memoryStream;
        request.ContentType = "application/json";
        return request;
    }
}
