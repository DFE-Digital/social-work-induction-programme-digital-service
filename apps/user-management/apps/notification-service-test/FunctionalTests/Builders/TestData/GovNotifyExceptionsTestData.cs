using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace DfeSwwEcf.NotificationService.Tests.FunctionalTests.Builders.TestData
{
    public class GovNotifyExceptionsTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            // Gov Notify throws Internal Server Error
            yield return NotificationTestDataBuilder
                .CreateNew()
                .WithEmailAddress("test@test.com")
                .WithTemplateId("2d6ad686-b5e7-466c-a810-60709b1b091c")
                .WithPersonalText("ABC123")
                .WithExpectedStatusCode(HttpStatusCode.InternalServerError)
                .BuildForGovNotifyException();

            // Gov Notify fails to authenticate (Unauthorized)
            yield return NotificationTestDataBuilder
                .CreateNew()
                .WithEmailAddress("test@test.com")
                .WithTemplateId("2d6ad686-b5e7-466c-a810-60709b1b091c")
                .WithPersonalText("ABC123")
                .WithExpectedStatusCode(HttpStatusCode.Unauthorized)
                .BuildForGovNotifyException();

            // Gov Notify rate limit exceeded(429)
            yield return NotificationTestDataBuilder
                .CreateNew()
                .WithEmailAddress("test@test.com")
                .WithTemplateId("2d6ad686-b5e7-466c-a810-60709b1b091c")
                .WithPersonalText("ABC123")
                .WithExpectedStatusCode(HttpStatusCode.TooManyRequests)
                .BuildForGovNotifyException();

            // Incorrect template ID
            yield return NotificationTestDataBuilder
                .CreateNew()
                .WithEmailAddress("test@test.com")
                .WithTemplateId("458a970f-e8c4-472c-b038-78fae46f2977")
                .WithPersonalText("ABC123")
                .WithExpectedStatusCode(HttpStatusCode.BadRequest)
                .BuildForGovNotifyException();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
