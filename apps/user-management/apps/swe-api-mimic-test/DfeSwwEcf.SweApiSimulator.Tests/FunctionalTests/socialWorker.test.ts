import { Send, GetRequest, LastResponse } from '@serenity-js/rest';
import {Ensure, equals, property, and} from '@serenity-js/assertions';
import { describe, it } from '@serenity-js/playwright-test';

process.env["NODE_TLS_REJECT_UNAUTHORIZED"] = "0";

describe('GET Social Worker Details', () => {
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
});


