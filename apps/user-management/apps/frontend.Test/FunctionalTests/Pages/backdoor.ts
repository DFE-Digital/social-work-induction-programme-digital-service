/// <reference types="@serenity-js/core" />

import { PageElement, By } from '@serenity-js/web';

export const coordinatorJoeBloggs = () =>
    PageElement.located(
        By.cssContainingText('label.govuk-radios__label','Joe Bloggs | Coordinator')
    ).describedAs('Label for coordinator Joe Bloggs');

export const logInAsWhoCaption = () =>
    PageElement.located(
        By.css('h1.govuk-fieldset__heading')
    ).describedAs('Who do you want to log in as caption');

export const logInButton = () =>
    PageElement.located(
        By.cssContainingText('button.govuk-button','Log in'),
    ).describedAs('Log in button');
