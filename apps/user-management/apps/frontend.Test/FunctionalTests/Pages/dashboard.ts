/// <reference types="@serenity-js/core" />

import { PageElement, By } from '@serenity-js/web';

export const dashboardTitle = () =>
    PageElement.located(
        By.css('h1.govuk-heading-l'),
    ).describedAs('Dashboard title');
