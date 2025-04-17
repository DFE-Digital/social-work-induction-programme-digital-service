using System.Net;
using FluentValidation.Results;

namespace DfeSwwEcf.NotificationService.Tests.FunctionalTests.Builders.TestData;

public class NotificationTestDataBuilder
{
    private string _emailAddress = null!;
    private string? _templateId;
    private string _personalText = null!;
    private HttpStatusCode _expectedStatusCode;
    private readonly List<ValidationFailure> _validationExpectedResponseBody = [];
    private string? _jsonExpectedResponseBody;

    public static NotificationTestDataBuilder CreateNew()
    {
        return new NotificationTestDataBuilder();
    }

    public NotificationTestDataBuilder WithEmailAddress(string emailAddress)
    {
        _emailAddress = emailAddress;
        return this;
    }

    public NotificationTestDataBuilder WithTemplateId(string templateId)
    {
        _templateId = templateId;
        return this;
    }

    public NotificationTestDataBuilder WithPersonalText(string personalText)
    {
        _personalText = personalText;
        return this;
    }

    public NotificationTestDataBuilder WithExpectedStatusCode(HttpStatusCode expectedStatusCode)
    {
        _expectedStatusCode = expectedStatusCode;
        return this;
    }

    public NotificationTestDataBuilder WithValidationFailure(
        string propertyName,
        string errorMessage,
        object attemptedValue,
        string errorCode,
        Dictionary<string, object> formattedMessagePlaceholderValues
    )
    {
        _validationExpectedResponseBody.Add(
            new ValidationFailure(propertyName, errorMessage, attemptedValue)
            {
                ErrorCode = errorCode,
                FormattedMessagePlaceholderValues = formattedMessagePlaceholderValues
            }
        );
        return this;
    }

    public NotificationTestDataBuilder WithJsonValidationFailure(string errorMessage)
    {
        _jsonExpectedResponseBody = errorMessage;
        return this;
    }

    // Build method for SendNotificationWithInValidData
    public object[] BuildForInvalidData()
    {
        return new object[]
        {
            _emailAddress,
            _templateId ?? Guid.Empty.ToString(),
            _personalText,
            _expectedStatusCode,
            _validationExpectedResponseBody
        };
    }

    // Build method for SendNotificationWithInvalidJson
    public object[] BuildForInvalidJson()
    {
        return new object[]
        {
            _emailAddress,
            _templateId ?? Guid.Empty.ToString(),
            _personalText,
            _expectedStatusCode,
            _jsonExpectedResponseBody!
        };
    }

    // Build method for SendNotificationWhenGovNotifyThrowsException
    public object[] BuildForGovNotifyException()
    {
        return new object[]
        {
            _emailAddress,
            _templateId ?? Guid.Empty.ToString(),
            _personalText,
            _expectedStatusCode
        };
    }
}
