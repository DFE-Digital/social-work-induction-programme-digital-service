import { Send, GetRequest, LastResponse } from '@serenity-js/rest';
import {Ensure, equals, property, and} from '@serenity-js/assertions';
import { describe, it } from '@serenity-js/playwright-test';

process.env["NODE_TLS_REJECT_UNAUTHORIZED"] = "0";

describe('GET Social Worker Details', () => {

    // Positive Test Case: Valid SWID
    it('should return the social worker details with status 200', async ({ actorCalled }) => {
        const actor = actorCalled('Alice');
        await actor.attemptsTo(
            Send.a(GetRequest.to('/SocialWorker/5604')),
            // Ensure the response status code is 200
            Ensure.that(LastResponse.status(), equals(200)),

            // Ensure the response content-type is application/json
            Ensure.that(
                LastResponse.header('content-type'),
                equals('application/json; charset=utf-8')
            ),

            // Ensure the response body contains the expected data
            Ensure.that(LastResponse.body(),
                and(
                    property('Registered Name', equals('Ralph Cormier')),
                    property('Registration Number', equals('SW5604')),
                    property('Status', equals('Registered')),
                    property('Town of employment', equals('Workington')),
                    property('Registered from', equals('2012-08-01T00:00:00')),
                    property('Registered until', equals('2024-11-30T00:00:00')),
                    property('Registered', equals(true))
                ))
        );
    });

    // Test Case 3: Negative Test - SWE ref (Unmatched SWID)
    it('should return 200 OK with invalid request message for unmatched SWID', async ({ actorCalled }) => {
        const actor = actorCalled('Alice');
        await actor.attemptsTo(
            Send.a(GetRequest.to('/SocialWorker/95')), // A
            // Ensure the response status code is 200
            Ensure.that(LastResponse.status(), equals(200)),
            // Ensure the response body contains the expected message
            Ensure.that(LastResponse.body(),
                equals('Invalid Request')
            )
        );
    });

    // Test Case 4: Negative Test - SWE ref (Null SWID)
    it('should return 400 Bad Request for null SWID', async ({ actorCalled }) => {
        const actor = actorCalled('Alice');
        await actor.attemptsTo(
            Send.a(GetRequest.to('/SocialWorker/')),
            // Ensure the response status code is 400
            Ensure.that(LastResponse.status(), equals(400)),
            // Ensure the response body contains the expected error message
            Ensure.that(LastResponse.body(),
                equals('Please provide non-null value')
            )
        );
    });

    // Test Case 5: Negative Test - SWE ref (SWID Greater Than Max Char)
    it('should return 500 Internal Server Error for too large SWID', async ({ actorCalled }) => {
        const actor = actorCalled('Alice');
        await actor.attemptsTo(
            Send.a(GetRequest.to('/SocialWorker/111222333444555666')), // Exceeds Int32 max size
            // Ensure the response status code is 500
            Ensure.that(LastResponse.status(), equals(500)),
            // Ensure the response body contains the expected error message
            Ensure.that(LastResponse.body(),
                equals('Internal server error: One or more error occurred. (Value was either too large or too small for an Int32.)')
            )
        );
    });

    // Test Case 6: Negative Test - SWE ref (Invalid SWID Format Non Int)
    it('should return 422 Unprocessable Entity for invalid SWID format', async ({ actorCalled }) => {
        const actor = actorCalled('Alice');
        await actor.attemptsTo(
            Send.a(GetRequest.to('/SocialWorker/SW')),
            // Ensure the response status code is 422
            Ensure.that(LastResponse.status(), equals(422)),
            // Ensure the response body contains the expected error message
            Ensure.that(LastResponse.body(),
                property('error', equals('Please enter valid integer value'))
            )
        );
    });

    // Test Case 7: Negative Test - SWE Number (Value with in range but Non-existent SWID)
    it('should return 404 Not Found for non-existent SWID', async ({ actorCalled }) => {
        const actor = actorCalled('Alice');
        await actor.attemptsTo(
            Send.a(GetRequest.to('/SocialWorker/99999999')),
            // Ensure the response status code is 404
            Ensure.that(LastResponse.status(), equals(404)),
            // Ensure the response body contains the expected error message
            Ensure.that(LastResponse.body(),
                equals('No SocialWorker found with this ID')
            )
        );
    });
});

