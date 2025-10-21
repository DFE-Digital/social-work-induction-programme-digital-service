import {Ensure, equals} from '@serenity-js/assertions';
import { Task } from '@serenity-js/core';
import {Click, isVisible, Navigate, Scroll, Text} from '@serenity-js/web';
import {continueToLogin, serviceNameHeading} from "../Pages";
import {dashboardTitle} from "../Pages/dashboard";

export const authenticateAsAdmin = () =>
    Task.where(`#actor log in as the test admin user`,
        Navigate.to('/'),
        Ensure.that(serviceNameHeading(), isVisible()),
        Ensure.that(Text.of(serviceNameHeading()), equals('Social Work Induction Programme')),
        Scroll.to(continueToLogin()),
        Ensure.that(continueToLogin(), isVisible()),
        Click.on(continueToLogin()),
        Ensure.that(dashboardTitle(), isVisible()),
        Ensure.that(Text.of(dashboardTitle()), equals('Dashboard')),
    );

