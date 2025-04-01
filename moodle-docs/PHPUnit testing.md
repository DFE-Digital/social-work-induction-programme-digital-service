# PHPUnit Moodle testing

PHPUnit is an advanced unit testing framework for PHP. It is installed as Composer dependency and is not part of Moodle installation. To run PHPUnit tests, you have to install it on your development computer or test server.

## Setting up PHPUnit locally with ddev

To install and run PHPUnit tests in a local ddev environment, the following steps have been taken. These are based on the 
[PHPUnit Moodle documentation](https://moodledev.io/general/development/tools/phpunit).

1. In VSCode, Moodle config.php file amended to include: 

`$CFG->phpunit_prefix = 'phpu_';`
`$CFG->phpunit_dataroot = '/home/example/phpu_moodledata';`

2. In a terminal, created directory for PHPunit data by running the below from moodle_app folder:

`mkdir phpunit_moodle_data`

3. Checked that `phpunit` is available:

`ddev composer show phpunit/phpunit`

4. Ran the following:

`ddev ssh`
`cd public`

5. Checked that required `en_AU` locale is in the `/etc/locale.gen` file by running the following from the public folder:

`locale -a | grep en_AU`
Nothing was returned. Added `en_AU.UTF-8 UTF-8` line to `/etc/locale.gen`:

`sudo nano /etc/locale.gen`

Generated the locales by running:

`sudo locale-gen`

6. Ran the PHPunit init script (from `public` or amend the path accordingly):

`php admin/tool/phpunit/cli/init.php`

7. Executed a test (from `public` or amend the path accordingly):

`vendor/bin/phpunit --filter test_get_registry_metadata_count`

## Running tests

To run PHPUnit tests after the ddev environment was stopped/restarted, you have to run the phpunit init script again: 

`php admin/tool/phpunit/cli/init.php`

You may also have to go through step 5 documented above.

To list the suites of tests available:

`php public/vendor/bin/phpunit --list-suites`

For more instructions on options you can use when running tests, see [PHPUnit Moodle documentation](https://moodledev.io/general/development/tools/phpunit)

## Installation gotchas

* **Not using the correct PHP version**

    - Make sure DDEV's PHP version matches your Moodle version. Use ddev config or .ddev/config.yaml to adjust.

    - Run ddev restart after changing PHP version.

* **Database not initialized for testing**

PHPUnit tests require a separate test database. You must run:

`ddev ssh`
`php public/admin/tool/phpunit/cli/init.php`

This wipes existing test data and sets up a new test DB.

* **Missing config for test DB**

Ensure your config.php includes:
    
`$CFG->phpunit_prefix = 'phpu_';`

DDEV uses db as the DB host, not localhost.

* **Code not placed in the right directory**

    - Tests must live in the correct tests/ directory (e.g., mod/yourplugin/tests/).

    - Files must be named like yourtest_test.php and classes like yourtest_testcase.

* **PHPUnit not installed globally**

Use the version bundled with Moodle:

`ddev ssh`
`vendor/bin/phpunit [testpath]`

* **Forgetting to install dev dependencies**

If running in a custom setup:

`ddev ssh`
`composer install`

* **Browser tests (Behat) confused with PHPUnit**

Behat is for UI testing, PHPUnit is for backend. Don’t mix commands/config.

## Next steps

To run PHPUnit tests in a CI/CD pipeline, these are the envisaged next steps:

* Run setup commands before executing PHPUnit by adding a setup step

    - Run `sudo locale-gen en_AU.UTF-8`

    - Initialise PHPUnit with `php public/admin/tool/phpunit/cli/init.php`

    - Ensure the database is set up before running tests

* Use [GitHub - php-actions/phpunit: Run PHPUnit tests in Github Actions](https://github.com.mcas.ms/php-actions/phpunit) to run the tests

The following is potentially a useful resources on Moodle CI:
[Add support for running database replicas](https://github.com.mcas.ms/moodlehq/moodle-docker/issues/217)

## Recommended core test suites to consider running

core_phpunit_testsuite

core_test_testsuite

core_ddl_testsuite

core_dml_testsuite

core_testsuite

core_external_testsuite

core_favourites_testsuite

core_form_testsuite

core_files_testsuite

core_filter_testsuite

core_role_testsuite

core_cohort_testsuite

core_grades_testsuite

core_analytics_testsuite

core_availability_testsuite

core_backup_testsuite

core_badges_testsuite

core_blog_testsuite

core_customfield_testsuite

core_iplookup_testsuite

core_course_testsuite

core_courseformat_testsuite

core_privacy_testsuite

core_question_testsuite

core_cache_testsuite

core_calendar_testsuite

core_enrol_testsuite

core_group_testsuite

core_message_testsuite

core_notes_testsuite

core_tag_testsuite

core_rating_testsuite

core_repository_testsuite

core_userkey_testsuite

core_user_testsuite

core_webservice_testsuite

core_mnet_testsuite

core_completion_testsuite

core_comment_testsuite

core_search_testsuite

core_competency_testsuite

core_my_testsuite

core_auth_testsuite

core_block_testsuite

core_login_testsuite

core_plagiarism_testsuite

core_portfolio_testsuite

core_editor_testsuite

core_rss_testsuite

core_table_testsuite

core_h5p_testsuite

core_xapi_testsuite

core_contentbank_testsuite

core_payment_testsuite

core_reportbuilder_testsuite

core_adminpresets_testsuite

core_admin_testsuite

core_communication_testsuite

core_ai_testsuite

core_sms_testsuite
