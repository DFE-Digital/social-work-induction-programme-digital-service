import { describe, it } from '@serenity-js/playwright-test';
import {authenticateAsAdmin} from "./Tasks/login";

describe('Social Work Induction Programme', () => {

    describe('when the user lands on the service page', () => {
        it('should be able to continue to log in from the service page', async ({ actor }) => {
            await actor.attemptsTo(
                authenticateAsAdmin(),
            );
        });
    });
});
