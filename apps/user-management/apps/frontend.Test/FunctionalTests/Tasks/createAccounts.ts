import {Task} from "@serenity-js/core";
import { faker } from '@faker-js/faker';
import {authenticateAsJoeBloggsCoordinator} from "./login";
import {Click, Enter, isVisible, Scroll} from "@serenity-js/web";
import {
    addAccountButton,
    addSomeoneButton,
    confirmAddUserButton,
    confirmAddUserCaption,
    emailInput,
    enterDetailsCaption,
    firstnameInput,
    lastnameInput,
    notificationBannerTitle,
    userAccountListing,
} from "../Pages";
import {Ensure, equals} from "@serenity-js/assertions";
import {navigateToManageAccounts, setAccountTypeTo} from "./accountManagement";

const startAddUser = () =>
    Task.where('#actor starts the \'add user\' journey',
        Scroll.to(addSomeoneButton()),
        Ensure.that(addSomeoneButton(),isVisible()),
        Click.on(addSomeoneButton())
    );

const enterAndSubmitUserDetails = (firstName: string, lastName: string, email: string) =>
    Task.where('#actor enters user details and submits',
        Ensure.that(enterDetailsCaption(),isVisible()),
        Ensure.that(enterDetailsCaption().text(), equals('Enter their details')),
        Ensure.that(firstnameInput(),isVisible()),
        Enter.theValue(firstName).into(firstnameInput()),
        Ensure.that(lastnameInput(),isVisible()),
        Enter.theValue(lastName).into(lastnameInput()),
        Ensure.that(emailInput(),isVisible()),
        Enter.theValue(email).into(emailInput()),
        Scroll.to(addAccountButton()),
        Ensure.that(addAccountButton(),isVisible()),
        Click.on(addAccountButton())
    );

const confirmUserDetails = () =>
    Task.where('#actor confirms user details',
        Ensure.that(confirmAddUserCaption(),isVisible()),
        Ensure.that(confirmAddUserCaption().text(), equals('Check user details before adding them')),
        Scroll.to(confirmAddUserButton()),
        Ensure.that(confirmAddUserButton(),isVisible()),
        Click.on(confirmAddUserButton())
    );

const checkUserListing = (fullName: string) =>
    Task.where(`#actor confirms user details for ${fullName}`,
        Ensure.that(notificationBannerTitle(),isVisible()),
        Scroll.to(userAccountListing(fullName)),
        Ensure.that(userAccountListing(fullName),isVisible())
    );

// Create social worker account
const socialWorkerFirstName = faker.person.firstName();
const socialWorkerLastName = faker.person.lastName();
const socialWorkerEmail = faker.internet.email({firstName: socialWorkerFirstName, lastName: socialWorkerLastName});
export const socialWorkerFullName = socialWorkerFirstName + ' ' + socialWorkerLastName;
export const createEarlyCareerSocialWorker = () =>
    Task.where(`#actor adds new user with the role of early career social worker`,
        authenticateAsJoeBloggsCoordinator(),
        navigateToManageAccounts(),
        startAddUser(),
        // Select early career social worker and continue
        setAccountTypeTo('IsStaff',[],false),
        enterAndSubmitUserDetails(socialWorkerFirstName, socialWorkerLastName, socialWorkerEmail),
        confirmUserDetails(),
        checkUserListing(socialWorkerFullName)
    );

// Create coordinator account
const coordinatorFirstName = faker.person.firstName();
const coordinatorLastName = faker.person.lastName();
const coordinatorEmail = faker.internet.email({firstName: coordinatorFirstName, lastName: coordinatorLastName});
export const coordinatorFullName = coordinatorFirstName + ' ' + coordinatorLastName;
export const createCoordinator = () =>
    Task.where(`#actor adds new user with the role of coordinator`,
        authenticateAsJoeBloggsCoordinator(),
        navigateToManageAccounts(),
        startAddUser(),
        // Select coordinator
        setAccountTypeTo('IsStaff-2', ['Coordinator'], false),
        enterAndSubmitUserDetails(coordinatorFirstName, coordinatorLastName, coordinatorEmail),
        confirmUserDetails(),
        checkUserListing(coordinatorFullName)
    );

// Create assessor account
const assessorFirstName = faker.person.firstName();
const assessorLastName = faker.person.lastName();
const assessorEmail = faker.internet.email({firstName: assessorFirstName, lastName: assessorLastName});
export const assessorFullName = assessorFirstName + ' ' + assessorLastName;
export const createAssessor = () =>
    Task.where(`#actor adds new user with the role of assessor`,
        authenticateAsJoeBloggsCoordinator(),
        navigateToManageAccounts(),
        startAddUser(),
        // Select assessor
        setAccountTypeTo('IsStaff-2', ['Assessor'], false),
        enterAndSubmitUserDetails(assessorFirstName, assessorLastName, assessorEmail),
        confirmUserDetails(),
        checkUserListing(assessorFullName)
    );

// Create assessor coordinator account
const assessorCoordinatorFirstName = faker.person.firstName();
const assessorCoordinatorLastName = faker.person.lastName();
const assessorCoordinatorEmail = faker.internet.email({firstName: assessorCoordinatorFirstName, lastName: assessorCoordinatorLastName});
export const assessorCoordinatorFullName = assessorCoordinatorFirstName + ' ' + assessorCoordinatorLastName;
export const createAssessorCoordinator = () =>
    Task.where(`#actor adds new user with the roles of assessor and coordinator`,
        authenticateAsJoeBloggsCoordinator(),
        navigateToManageAccounts(),
        startAddUser(),
        // Select assessor and coordinator
        setAccountTypeTo('IsStaff-2', ['Assessor', 'Coordinator'], false),
        enterAndSubmitUserDetails(assessorCoordinatorFirstName, assessorCoordinatorLastName, assessorCoordinatorEmail),
        confirmUserDetails(),
        checkUserListing(assessorCoordinatorFullName)
    );
