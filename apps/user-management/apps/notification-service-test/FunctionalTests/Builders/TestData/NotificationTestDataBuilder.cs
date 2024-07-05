using System.Net;
using FluentValidation.Results;

namespace DfeSwwEcf.NotificationService.Tests.FunctionalTests.Builders.TestData;

public class NotificationTestDataBuilder
{
    private string _emailAddress = null!;
    private Guid _templateId;
    private string _personalText = null!;
    private HttpStatusCode _expectedStatusCode;
    private List<ValidationFailure> _expectedResponseBody = new List<ValidationFailure>();

    public static NotificationTestDataBuilder CreateNew()
    {
        return new NotificationTestDataBuilder();
    }

    public NotificationTestDataBuilder WithEmailAddress(string emailAddress)
    {
        _emailAddress = emailAddress;
        return this;
    }

    public NotificationTestDataBuilder WithTemplateId(Guid templateId)
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

    public NotificationTestDataBuilder WithValidationFailure(string propertyName, string errorMessage,
        object attemptedValue, string errorCode, Dictionary<string, object> formattedMessagePlaceholderValues)
    {
        _expectedResponseBody.Add(new ValidationFailure(propertyName, errorMessage, attemptedValue)
        {
            ErrorCode = errorCode,
            FormattedMessagePlaceholderValues = formattedMessagePlaceholderValues
        });
        return this;
    }

    public object[] Build()
    {
        return new object[] { _emailAddress, _templateId, _personalText, _expectedStatusCode, _expectedResponseBody };
    }
}
