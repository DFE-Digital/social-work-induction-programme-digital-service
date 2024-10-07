/// <reference types="@serenity-js/core" />
import { PageElement, By } from '@serenity-js/web';

// Page elements for the Manage Accounts page
export const manageAccountsLink = () =>
    PageElement.located(
        By.css('a.dfe-header__navigation-link[href="/manage-accounts"]')
    ).describedAs('Manage accounts');

export const manageAccountsCaption = () =>
    PageElement.located(
        By.css('caption.govuk-table__caption.govuk-table__caption--l')
    ).describedAs('Manage accounts caption');

export const addSomeoneButton = () =>
    PageElement.located(
        By.css('a.govuk-button[href="/manage-accounts/select-account-type?handler=New"]')
    ).describedAs('Add someone button');

export const viewButton = (row: number) =>
    PageElement.located(
        By.css(`table.govuk-table tbody tr:nth-child(${row}) a[href*="/view-account-details"]`)
    ).describedAs(`View button in row ${row}`);

export const changeAccountTypeButton = () =>
    PageElement.located(
        By.css('a[href*="/manage-accounts/select-account-type"]')
    ).describedAs('Change account type button');

export const whoDoYouWantToAddCaption = () =>
    PageElement.located(
        By.css('h1.govuk-fieldset__heading')
    ).describedAs('Who do you want to add caption');

export const updatedAccountTypeValue = () =>
    PageElement.located(
        By.xpath('//dl[@class="govuk-summary-list"]//div[@class="govuk-summary-list__row"][dt[contains(text(), "Account type")]]//dd[@class="govuk-summary-list__value"]')
    ).describedAs('Value for the key "Account type"');

export const whatDoTheyNeedToDoCaption = () =>
    PageElement.located(
        By.css('h1.govuk-fieldset__heading')
    ).describedAs('What do they need to do caption');

export const assessorCheckbox = () =>
    PageElement.located(
        By.css('label.govuk-checkboxes__label.govuk-label[for="SelectedAccountTypes"]')
    ).describedAs('Assess work and provide feedback to early career social workers');

export const coordinatorCheckbox = () =>
    PageElement.located(
        By.css('label.govuk-checkboxes__label.govuk-label[for="SelectedAccountTypes-2"]')
    ).describedAs('Add, remove and manage accounts for your organisation');

export const continueButton = () =>
    PageElement.located(
        By.css('button.govuk-button')
    ).describedAs('Continue button');

export const saveChangesButton = () =>
    PageElement.located(
        By.css('button.govuk-button')
    ).describedAs('Save changes button');

export const accountTypeLabelElement = (accountTypeLabel: string) =>
    PageElement.located(By.css(`label.govuk-radios__label.govuk-label[for="${accountTypeLabel}"]`))
        .describedAs(accountTypeLabel);

export const userAccountListing = (fullname: string) =>
    PageElement.located(
        By.cssContainingText('th[data-test-class=\'account-name\']',`${fullname}`)
    ).describedAs('User account listing with given name');

export const userAccountStatus = (fullname: string) =>
    PageElement.located(
        By.xpath(`//th[@data-test-class=\'account-name\'][normalize-space()="${fullname}"]/following-sibling::td/child::strong[@data-test-class=\'account-status\']`)
    ).describedAs('Account status for given user');

export const viewGivenUser = (fullname: string) =>
    PageElement.located(
        By.css(`a[title="View details of ${fullname}"]`),
    ).describedAs(`View details of given user`);

export const notificationBannerTitle = () =>
    PageElement.located(
        By.css('.govuk-notification-banner__title')
    ).describedAs('Title of notification banner');

// Add user elements
export const enterDetailsCaption = () =>
    PageElement.located(
        By.css('form[data-test-id=\'add-user-form\'] .govuk-fieldset__heading')
    ).describedAs('Header of add user details page');

export const firstnameInput = () =>
    PageElement.located(
        By.css('form[data-test-id=\'add-user-form\'] #FirstName')
    ).describedAs('Input field for first name');

export const lastnameInput = () =>
    PageElement.located(
        By.css('form[data-test-id=\'add-user-form\'] #LastName')
    ).describedAs('Input field for last name');

export const emailInput = () =>
    PageElement.located(
        By.css('form[data-test-id=\'add-user-form\'] #Email')
    ).describedAs('Input field for email');

export const addAccountButton = () =>
    PageElement.located(
        By.css('form[data-test-id=\'add-user-form\'] button[data-test-id=\'add-account\']')
    ).describedAs('Add user button');

// Confirm user details elements
export const confirmAddUserCaption = () =>
    PageElement.located(
        By.css('form[data-test-id=\'confirm-details\'] h1')
    ).describedAs('Header of confirm user details page');

export const confirmAddUserButton = () =>
    PageElement.located(
        By.css('form[data-test-id=\'confirm-details\'] button[type=\'submit\']')
    ).describedAs('Add user confirmation button');

// Unlink elements
export const unlinkAccountAnchorLink = () =>
    PageElement.located(
        By.css('[data-test-id=\'unlink\']')
    ).describedAs('Unlink user anchor link');

export const confirmUnlinkCaption = () =>
    PageElement.located(
        By.css('.govuk-heading-l[data-test-id=\'unlink-caption\']')
    ).describedAs('Unlink user confirmation page title');

export const unlinkConfirmButton = () =>
    PageElement.located(
        By.cssContainingText('form[data-test-id=\'unlink-form\'] button.govuk-button','Confirm')
    ).describedAs('Confirm unlink button');

// Link elements
export const linkAccountAnchorLink = () =>
    PageElement.located(
        By.css('[data-test-id=\'link\']')
    ).describedAs('Link user anchor');

export const confirmLinkCaption = () =>
    PageElement.located(
        By.css('.govuk-heading-l[data-test-id=\'link-caption\']')
    ).describedAs('Link user confirmation page title');

export const linkConfirmButton = () =>
    PageElement.located(
        By.cssContainingText('form[data-test-id=\'link-form\'] button.govuk-button','Confirm')
    ).describedAs('Confirm link button');
