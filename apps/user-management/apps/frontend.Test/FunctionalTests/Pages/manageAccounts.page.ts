/// <reference types="@serenity-js/core" />

import { PageElement, By } from '@serenity-js/web';

export const manageAccountsLink = () =>
    PageElement.located(
        By.css('a.dfe-header__navigation-link[href="/manage-accounts"]'),
    ).describedAs('Manage accounts link');

export const manageAccountsCaption = () =>
    PageElement.located(
        By.css('caption.govuk-table__caption.govuk-table__caption--l'),
    ).describedAs('Manage accounts caption');

export const addSomeoneButton = () =>
    PageElement.located(
        By.css('a.govuk-button[href="/manage-accounts/select-account-type"]'),
    ).describedAs('Add someone button');
