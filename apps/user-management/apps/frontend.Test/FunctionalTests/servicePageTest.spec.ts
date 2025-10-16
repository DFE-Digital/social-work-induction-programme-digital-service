import { Ensure, equals, isPresent } from '@serenity-js/assertions';
import { describe, it } from '@serenity-js/playwright-test';
import { Click, isVisible, Navigate, Text } from '@serenity-js/web';
import { serviceNameHeading, manageAccountsCaption, manageAccountsLink, addSomeoneButton } from './Pages';
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
