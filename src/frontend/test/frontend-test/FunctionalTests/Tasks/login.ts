import { Ensure } from '@serenity-js/assertions';
import { Task } from '@serenity-js/core';
import {Click, isVisible, Navigate, Scroll} from '@serenity-js/web';
import {continueToLogin, manageAccountsLink} from "../Pages";
import {coordinatorJoeBloggs, logInAsWhoCaption, logInButton} from "../Pages/backdoor";

export const authenticateAsJoeBloggsCoordinator = () =>
    Task.where(`#actor log in as the coordinator Joe Bloggs`,
        Navigate.to('/'),
        Ensure.that(continueToLogin(), isVisible()),
        Click.on(continueToLogin()),
        Ensure.that(logInAsWhoCaption(), isVisible()),
        Ensure.that(coordinatorJoeBloggs(), isVisible()),
        Click.on(coordinatorJoeBloggs()),
        Scroll.to(logInButton()),
        Ensure.that(logInButton(), isVisible()),
        Click.on(logInButton()),
        Ensure.that(manageAccountsLink(), isVisible()),
    );

