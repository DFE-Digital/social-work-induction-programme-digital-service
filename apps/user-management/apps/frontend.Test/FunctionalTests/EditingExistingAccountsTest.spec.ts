import { describe, it } from '@serenity-js/playwright-test';
import './Pages/backdoor';
import { setAccountTypeTo, navigateToManageAccounts, verifyUpdatedAccountType, viewAccount } from './Tasks/accountManagement';
import {authenticateAsJoeBloggsCoordinator} from "./Tasks/login";

describe('Manage Accounts - Change Account Type', () => {

    it('should allow Alice to change the account type to Early career social worker', async ({ actor }) => {
        await actor.attemptsTo(
            authenticateAsJoeBloggsCoordinator(),
            navigateToManageAccounts(),
            viewAccount(1),
            setAccountTypeTo('IsStaff', [], true),
            verifyUpdatedAccountType('Early career social worker')
        );
    });

    it('should allow Alice to change the account type to A staff member supporting the PQP programme - assessor', async ({ actor }) => {
        await actor.attemptsTo(
            authenticateAsJoeBloggsCoordinator(),
            navigateToManageAccounts(),
            viewAccount(1),
            setAccountTypeTo('IsStaff-2', ['Assessor'], true),
            verifyUpdatedAccountType('Assessor')
        );
    });

    it('should allow Alice to change the account type to A staff member supporting the PQP programme - coordinator', async ({ actor }) => {
        await actor.attemptsTo(
            authenticateAsJoeBloggsCoordinator(),
            navigateToManageAccounts(),
            viewAccount(1),
            setAccountTypeTo('IsStaff-2', ['Coordinator'], true),
            verifyUpdatedAccountType('Coordinator')
        );
    });

    it('should allow Alice to change the account type to A staff member supporting the PQP programme - Assessor, Coordinator', async ({ actor }) => {
        await actor.attemptsTo(
            authenticateAsJoeBloggsCoordinator(),
            navigateToManageAccounts(),
            viewAccount(1),
            setAccountTypeTo('IsStaff-2', ['Assessor', 'Coordinator'], true),
            verifyUpdatedAccountType('Assessor, Coordinator')
        );
    });
});
