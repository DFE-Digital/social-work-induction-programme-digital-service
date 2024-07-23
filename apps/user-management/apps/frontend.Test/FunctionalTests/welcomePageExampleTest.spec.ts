import { Ensure, equals, isPresent } from '@serenity-js/assertions';
import { describe, it } from '@serenity-js/playwright-test';
import { Click, isVisible, Navigate, Text } from '@serenity-js/web';
import { welcomeMessage, manageAccountsCaption, manageAccountsLink, addSomeoneButton } from './Pages';

describe('Early Career Framework Service', () => {

    describe('when the user navigates through the application', () => { 
        it('should navigate through the application and check elements', async ({ actor }) => {
            await actor.attemptsTo(
                Navigate.to('/'),

                Ensure.that(welcomeMessage(), isVisible()),
                Ensure.that(Text.of(welcomeMessage()), equals('Welcome to the post qualifying pathway digital service')),

                Ensure.that(manageAccountsLink(), isVisible()),
                Click.on(manageAccountsLink()),

                Ensure.that(manageAccountsCaption(), isVisible()),
                Ensure.that(Text.of(manageAccountsCaption()), equals('Manage accounts')),
                Ensure.that(addSomeoneButton(), isPresent()),
            );
        });
    });
});
