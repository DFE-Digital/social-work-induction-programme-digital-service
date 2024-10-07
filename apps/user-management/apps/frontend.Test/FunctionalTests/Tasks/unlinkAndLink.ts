import {Task} from "@serenity-js/core";
import {Click, isVisible, Scroll, Text} from "@serenity-js/web";
import {
    confirmLinkCaption,
    confirmUnlinkCaption, linkAccountAnchorLink, linkConfirmButton,
    manageAccountsCaption,
    notificationBannerTitle,
    unlinkAccountAnchorLink,
    unlinkConfirmButton, userAccountStatus
} from "../Pages";
import {Ensure, equals, not} from "@serenity-js/assertions";
import {socialWorkerFullName} from "./createAccounts";

// Select 'unlink' option on user details page
export const selectUnlink = () =>
    Task.where('#actor selects option to unlink user from organisation',
        Scroll.to(unlinkAccountAnchorLink()),
        Ensure.that(unlinkAccountAnchorLink(), isVisible()),
        Click.on(unlinkAccountAnchorLink()),
    );

// Confirm 'unlink' on dedicated page
export const confirmUnlink = () =>
    Task.where('#actor confirms unlink action',
        Ensure.that(confirmUnlinkCaption(), isVisible()),
        Ensure.that(Text.of(confirmUnlinkCaption()), equals('Are you sure you want to unlink?')),
        Ensure.that(unlinkConfirmButton(), isVisible()),
        Click.on(unlinkConfirmButton()),
    );

// Check unlink account changes on 'manage accounts' page
export const checkUnlinkChanges = (userName: string) =>
    Task.where(`#actor checks that given user ${userName} has been unlinked`,
        Ensure.that(manageAccountsCaption(), isVisible()),
        Ensure.that(notificationBannerTitle(),isVisible()),
        Ensure.that(Text.of(notificationBannerTitle()), equals('Account was successfully unlinked from this organisation')),
        Ensure.that(Text.of(userAccountStatus(userName)), equals('Not linked')),
    );

// Select 'link' option on user details page
export const selectLink = () =>
    Task.where('#actor selects option to link user to organisation',
        Scroll.to(linkAccountAnchorLink()),
        Ensure.that(linkAccountAnchorLink(), isVisible()),
        Click.on(linkAccountAnchorLink()),
    );

// Confirm 'link' on dedicated page
export const confirmLink = () =>
    Task.where('#actor confirms link action',
        Ensure.that(confirmLinkCaption(), isVisible()),
        Ensure.that(Text.of(confirmLinkCaption()), equals('Are you sure you want to link?')),
        Ensure.that(linkConfirmButton(), isVisible()),
        Click.on(linkConfirmButton()),
    );

// Check link account changes on 'manage accounts' page
export const checkLinkChanges = (userName: string) =>
    Task.where(`#actor checks that given user ${userName} has been linked`,
        Ensure.that(manageAccountsCaption(), isVisible()),
        Ensure.that(notificationBannerTitle(),isVisible()),
        Ensure.that(Text.of(notificationBannerTitle()), equals('Account was successfully linked to this organisation')),
        Ensure.that(Text.of(userAccountStatus(userName)), not(equals('Not linked')))
    );

