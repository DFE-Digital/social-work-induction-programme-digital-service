using System.Collections;
using System.Net;

namespace DfeSwwEcf.NotificationService.Tests.FunctionalTests.Builders.TestData
{
    public class JsonExceptionsTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            // Invalid JSON format missing required feilds
            yield return NotificationTestDataBuilder
                .CreateNew()
                .WithEmailAddress("test@test.com")
                .WithTemplateId("123")
                .WithExpectedStatusCode(HttpStatusCode.BadRequest)
                .WithJsonValidationFailure("Request body is not valid json")
                .BuildForInvalidJson();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
