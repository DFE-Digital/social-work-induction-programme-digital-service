import {Ensure, equals, isTrue} from '@serenity-js/assertions';
import {Check, Task} from '@serenity-js/core';
import {Click, isVisible, Text} from '@serenity-js/web';
import {
    manageAccountsLink, manageAccountsCaption, viewButton, changeAccountTypeButton,
    whoDoYouWantToAddCaption, accountTypeLabelElement, whatDoTheyNeedToDoCaption, continueButton, updatedAccountTypeValue,
    assessorCheckbox, coordinatorCheckbox
} from "../Pages/manageAccounts.page";

// Update all page element references to be function calls
export const navigateToManageAccounts = () =>
    Task.where(`#actor navigates to Manage Accounts`,
        Click.on(manageAccountsLink()),
        Ensure.that(manageAccountsCaption(), isVisible())
    );

export const viewAccount = (row: number) =>
    Task.where(`#actor views the account in row ${row}`,
        Click.on(viewButton(row)),
        Ensure.that(changeAccountTypeButton(), isVisible())
    );

export const setAccountTypeTo = (accountTypeLabel: string, checkboxLabels: string[] = [], update: boolean) =>
    Task.where(`#actor sets account type to ${accountTypeLabel}`,
        // Change account type only if the action is to update user details
        Check.whether(update,isTrue()).andIfSo(Click.on(changeAccountTypeButton())),
        Ensure.that(whoDoYouWantToAddCaption(), isVisible()),
        Click.on(accountTypeLabelElement(accountTypeLabel)),
        Ensure.that(continueButton(), isVisible()),
        Click.on(continueButton()),
        ...(accountTypeLabel === 'IsStaff-2' ? [
            Ensure.that(whatDoTheyNeedToDoCaption(), isVisible()),
            ...checkboxLabels.map(label => Click.on(getCheckboxElement(label))),
            Click.on(continueButton()),
        ] : [])
    );

const getCheckboxElement = (label: string) => {
    switch (label) {
        case 'Assessor':
            return assessorCheckbox();
        case 'Coordinator':
            return coordinatorCheckbox();
        default:
            throw new Error(`Unknown checkbox label: ${label}`);
    }
};

export const verifyUpdatedAccountType = (expectedType: string) =>
    Task.where(`#actor verifies the account type is ${expectedType}`,
        Ensure.that(updatedAccountTypeValue(), isVisible()),
        Ensure.that(Text.of(updatedAccountTypeValue()), equals(expectedType))
    );
