/// <reference types="@serenity-js/core" />

import { PageElement, By } from '@serenity-js/web';

export const serviceNameHeading = () =>
    PageElement.located(By.css('span.govuk-label--l'))
        .describedAs('Service name heading');

export const continueToLogin = () =>
    PageElement.located(By.css('a[href*="/sign-in"]'))
        .describedAs('Sign in with GOV.UK One Login');
