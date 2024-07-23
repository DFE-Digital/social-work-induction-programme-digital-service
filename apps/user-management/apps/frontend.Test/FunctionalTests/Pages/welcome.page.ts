/// <reference types="@serenity-js/core" />

import { PageElement, By } from '@serenity-js/web';

export const welcomeMessage = () =>
    PageElement.located(By.css('span.govuk-label--l'))
        .describedAs('Welcome Message');
