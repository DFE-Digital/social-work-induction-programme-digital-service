import {describe, it} from '@serenity-js/playwright-test';
import {Click} from '@serenity-js/web';
import {viewGivenUser} from "./Pages";
import {
    socialWorkerFullName,
    assessorCoordinatorFullName,
    assessorFullName,
    coordinatorFullName,
    createAssessor,
    createAssessorCoordinator,
    createCoordinator,
    createEarlyCareerSocialWorker
} from "./Tasks/createAccounts";
import {
    checkLinkChanges,
    checkUnlinkChanges,
    confirmLink,
    confirmUnlink,
    selectLink,
    selectUnlink
} from "./Tasks/unlinkAndLink";

/*
describe('When the user navigates to a created social worker\'s account', () => {
    it('they should be able to unlink them from the organisation and then relink them', async ({actor}) => {
        await actor.attemptsTo(
            createEarlyCareerSocialWorker(),
            // Unlink
            Click.on(viewGivenUser(socialWorkerFullName)),
            selectUnlink(),
            confirmUnlink(),
            checkUnlinkChanges(socialWorkerFullName),
            // Link
            Click.on(viewGivenUser(socialWorkerFullName)),
            selectLink(),
            confirmLink(),
            checkLinkChanges(socialWorkerFullName)
        );
    });
});

describe('When the user navigates to a created coordinator\'s account', () => {
    it('they should be able to unlink them from the organisation and then relink them', async ({actor}) => {
        await actor.attemptsTo(
            createCoordinator(),
            // Unlink
            Click.on(viewGivenUser(coordinatorFullName)),
            selectUnlink(),
            confirmUnlink(),
            checkUnlinkChanges(coordinatorFullName),
            // Link
            Click.on(viewGivenUser(coordinatorFullName)),
            selectLink(),
            confirmLink(),
            checkLinkChanges(coordinatorFullName)
        );
    });
});

describe('When the user navigates to a created assessor\'s account', () => {
    it('they should be able to unlink them from the organisation and then relink them', async ({actor}) => {
        await actor.attemptsTo(
            createAssessor(),
            // Unlink
            Click.on(viewGivenUser(assessorFullName)),
            selectUnlink(),
            confirmUnlink(),
            checkUnlinkChanges(assessorFullName),
            // Link
            Click.on(viewGivenUser(assessorFullName)),
            selectLink(),
            confirmLink(),
            checkLinkChanges(assessorFullName)
        );
    });
});

describe('When the user navigates to a created assessor coordinator\'s account', () => {
    it('they should be able to unlink them from the organisation and then relink them', async ({actor}) => {
        await actor.attemptsTo(
            createAssessorCoordinator(),
            // Unlink
            Click.on(viewGivenUser(assessorCoordinatorFullName)),
            selectUnlink(),
            confirmUnlink(),
            checkUnlinkChanges(assessorCoordinatorFullName),
            // Link
            Click.on(viewGivenUser(assessorCoordinatorFullName)),
            selectLink(),
            confirmLink(),
            checkLinkChanges(assessorCoordinatorFullName)
        );
    });
});
*/
