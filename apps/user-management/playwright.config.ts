import { devices } from '@playwright/test';
import type { PlaywrightTestConfig } from '@serenity-js/playwright-test';
import dotenv from 'dotenv';

// Load environment variables from .env file if available
dotenv.config();

// Define environment variables for spec directories
const FRONTEND_SPEC_DIR = process.env.FRONTEND_SPEC_DIR || './apps/frontend.Test/FunctionalTests';
const SWE_API_SPEC_DIR = process.env.SWE_API_SPEC_DIR || './apps/swe-api-mimic-test/DfeSwwEcf.SweApiSimulator.Tests/FunctionalTests';

// Check command line arguments to determine the project
const isFrontend = process.argv.includes('--project=frontend');
const isSweApi = process.argv.includes('--project=swe_api');

// Set specDirectory based on the project being run
const specDirectory = isFrontend ? FRONTEND_SPEC_DIR : isSweApi ? SWE_API_SPEC_DIR : FRONTEND_SPEC_DIR;

const config: PlaywrightTestConfig = {
    /* Maximum time one test can run for. */
    timeout: 30_000,
    expect: {
        /**
         * Maximum time expect() should wait for the condition to be met.
         * For example in `await expect(locator).toHaveText();`
         */
        timeout: 5000,
    },
    /* Run tests in files in parallel */
    fullyParallel: true,
    /* Fail the build on CI if you accidentally left test.only in the source code. */
    forbidOnly: !!process.env.CI,
    /* Retry on CI only */
    retries: process.env.CI ? 2 : 0,
    /* Opt out of parallel tests on CI. */
    workers: process.env.CI ? 1 : undefined,
    /* Reporter to use. See https://playwright.dev/docs/test-reporters */
    reporter: [
        ['line'],
        ['html', { open: 'never' }],
        ['@serenity-js/playwright-test', {
            crew: [
                '@serenity-js/console-reporter',
                ['@serenity-js/serenity-bdd', { specDirectory }],
                ['@serenity-js/core:ArtifactArchiver', { outputDirectory: `./test-results/serenity/${isFrontend ? 'frontend' : 'api'}` }],
            ],
        }],
    ],
    /* Shared settings for all the projects below. See https://playwright.dev/docs/api/class-testoptions. */
    use: {
        /* Base URL to use in actions like `await page.goto('/')`. */
        baseURL: 'http://localhost:5023',

        /* Set headless: false to see the browser window */
        headless: true,

        defaultActorName: 'Alice',
        crew: [
            // Take screenshots of failed Serenity/JS Activities, such as a failed assertion, or o failed interaction
            ['@serenity-js/web:Photographer', { strategy: 'TakePhotosOfFailures' }],
        ],

        /* Maximum time each action such as `click()` can take. Defaults to 0 (no limit). */
        actionTimeout: 0,

        /* Collect trace when retrying the failed test. See https://playwright.dev/docs/trace-viewer */
        trace: 'on-first-retry',
    },

    /* Configure projects for major browsers */
    projects: [
        {
            name: 'frontend',
            testDir: FRONTEND_SPEC_DIR,
            use: {
                ...devices['Desktop Chrome'],
                baseURL: 'http://localhost:5023',
            },
        },
        {
            name: 'swe_api',
            testDir: SWE_API_SPEC_DIR,
            use: {
                baseURL: 'https://localhost:7244',
            },
        },
    ],
};

export default config;
