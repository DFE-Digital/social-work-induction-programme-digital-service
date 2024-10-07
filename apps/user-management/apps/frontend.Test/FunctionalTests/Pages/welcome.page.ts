/// <reference types="@serenity-js/core" />

import { PageElement, By } from '@serenity-js/web';

export const welcomeMessage = () =>
    PageElement.located(By.css('span.govuk-label--l'))
        .describedAs('Welcome Message');

export const continueToLogin = () =>
    PageElement.located(By.css('a[href*="/debug/backdoor"]'))
        .describedAs('Continue to GOV.UK One Login link');
