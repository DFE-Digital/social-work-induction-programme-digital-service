# Behat Moodle testing

Moodle uses Behat, a php framework for automated functional testing, as part of a suite of testing tools.

Behat takes a set of Features, Scenarios, and Steps, and uses these to step through actions and test results using a web browser and a Webdriver which allows for the user interactions to be simulated.

## Setting up Behat locally with ddev

To install and run Behat tests in a local ddev environment, the following steps have been taken. These are based on the 
[Behat Moodle documentation](https://moodledev.io/general/development/tools/behat/running).

1. Started `wsl` from terminal on Windows and ran `ddev start` to start ddev containers and made sure the Moodle site works okay.

2. Ran `ddev ssh`

3. From root (`/var/www/html/`) cloned `moodle-browser-config` repo:

`git clone https://github.com/andrewnicols/moodle-browser-config`

4. Ran `mkdir behat_moodle_data` from the same directory as above.

5. In VSCode, updated Moodle `public/config.php` to include moodle-browser-config `init.php`. This needed to be above `lib/setup.php`. Also included the `$CFG settings` as per Moodle behat docs. 

`$CFG->behat_dataroot = '/var/www/html/behat_moodle_data';`
`$CFG->behat_wwwroot = 'http://moodle.ddev.site';`
`$CFG->behat_prefix = 'beh_';`

`require_once('/var/www/html/moodle-browser-config/init.php');`
`require_once(__DIR__ . '/lib/setup.php');`

Had to point `behat_wwwroot` to `http://moodle.ddev.site` as `http://host.docker.internal` and `127.0.0.1/localhost` threw an error in later steps ("Behat requirement not satisfied: http://host.docker.internal is not available, ensure you specified correct url and that the server is set up and started."). This is a temporary workaround as the ddev site then becomes a testing instance (clear all cookies and change the `behat_wwwroot` setting to access the moodle site normally after testing).

6. In VSCode, made a copy of `config_dist.php` file which is in the `moodle-browser-config` folder in the root, renamed to `config.php` and uncommented the `'selenium-url' ... ` line.

7. Back in the terminal, ran the behat init script from public (`/var/www/html/public`):

`php admin/tool/behat/cli/init.php`

8. Exited the ddev ssh (run `exit`) and installed selenium web driver, then ran `ddev restart`:

`ddev add-on get ddev/ddev-selenium-standalone-chrome`
`ddev restart`

9. Ran `ddev status` and saw the selenium service name and URL/ports.

In VSCode, updated the moodle-browser-config `config.php` file with the correct URL for the selenium web driver:

`...`
`return (object) [`
`    //`
`    // Configuration for Selenium.`
`    //`
`    // A default value for the selenium URL:`
`    'seleniumUrl' => 'http://selenium-chrome:4444/wd/hub',`
`...`

11. In the terminal, ran `ddev ssh` and the behat init script again to pick up config changes, changed dir to public and ran a test suite:

`php public/admin/tool/behat/cli/init.php`
`cd public`
`vendor/bin/behat --config /var/www/html/behat_moodle_data/behatrun/behat/behat.yml ``pwd``/mod/forum/tests/behat/private_replies.feature --profile=chrome`

12. Ran another set of tests which use Javascript. One failed for accessibility. May or may not be related to govuk theme.

`vendor/bin/behat --config /var/www/html/behat_moodle_data/behatrun/behat/behat.yml /var/www/html/public/lib/tests/behat/action_menu_subpanel.feature --profile=chrome`

13. In `.ddev/config.selenium-standalone-chrome.yaml` removed `--headless parameter` from the `MINK_DRIVER_ARGS_WEBDRIVER` environment variable. Accessed selenium in the browser on `https://moodle.ddev.site:7900/` to see the tests being executed while they ran (get url from step 9).

## Accessibility tests

The Moodle Behat tests also include accessibility checks.