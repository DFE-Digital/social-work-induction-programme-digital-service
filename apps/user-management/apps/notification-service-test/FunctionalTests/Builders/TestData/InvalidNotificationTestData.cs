using System.Collections;
using System.Net;

namespace DfeSwwEcf.NotificationService.Tests.FunctionalTests.Builders.TestData
{
    public class InvalidNotificationTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return NotificationTestDataBuilder.CreateNew()
                .WithEmailAddress("testtest.com")
                .WithTemplateId(new Guid("2d6ad686-b5e7-466c-a810-60709b1b091c"))
                .WithPersonalText("ABC123")
                .WithExpectedStatusCode(HttpStatusCode.BadRequest)
                .WithValidationFailure(
                    "EmailAddress",
                    "'Email Address' is not a valid email address.",
                    "testtest.com",
                    "EmailValidator",
                    new Dictionary<string, object>
                    {
                        { "PropertyName", "Email Address" },
                        { "PropertyValue", "testtest.com" },
                        { "PropertyPath", "EmailAddress" }
                    }
                )
                .Build();

            yield return NotificationTestDataBuilder.CreateNew()
                .WithEmailAddress("test@test.com")
                .WithTemplateId(Guid.Empty)
                .WithPersonalText("ABC123")
                .WithExpectedStatusCode(HttpStatusCode.BadRequest)
                .WithValidationFailure(
                    "TemplateId",
                    "'Template Id' must not be empty.",
                    Guid.Empty.ToString(),
                    "NotEmptyValidator",
                    new Dictionary<string, object>
                    {
                        { "PropertyName", "Template Id" },
                        { "PropertyValue", Guid.Empty.ToString() },
                        { "PropertyPath", "TemplateId" }
                    }
                )
                .Build();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
