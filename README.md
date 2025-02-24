# Social Work Induction Programme (SWIP) digital service

This repository houses the core digital service for the Social Work Induction programme in children's social care. The digital service is based on [Moodle LMS](https://moodle.org).

## Local development setup
The `moodle-docker` image in this repository builds on the [official Moodle PHP-apache image](https://github.com/moodlehq/moodle-php-apache). 
The configuration files in the .ddev directory provide a consistent local environment for running Moodle. Custom DDEV commands have been created to automate common tasks such as installing Moodle and setting up the govuk theme.

The primary custom command, install-moodle, bundles all installation steps into a single command. It performs the following tasks:

- Runs Moodle’s CLI installer.
- Downloads and installs the govuk theme into the correct directory.
- Updates config.php to set the default theme to govuk.
- Purges Moodle caches so that changes take effect immediately.
---
### Windows Setup
You need to clone this repository into your user directory in the WSL directory. This is usually found in `\\wsl.localhost\Ubuntu\home\{USERNAME}`. 

### DDEV Installation
Follow the instructions at [DDEV](https://ddev.readthedocs.io/en/stable/users/install/ddev-installation) for your operating system.

---
### Running the service
After configuring your project with DDEV, follow these steps from this project’s root directory:

Configure DDEV with both the composer root and document root set to public:

- `ddev config --composer-root=public --docroot=public --webserver-type=apache-fpm --database=postgres:14 --project-name=moodle`

Start your DDEV environment:

- `ddev start`

Use Composer to create a new Moodle installation:

- `ddev composer create moodle/moodle`

Use the custom DDEV command to perform the full installation, theme import, configuration update, and cache purge:

- `ddev install-moodle`

Finally, launch your Moodle site in your default web browser:

- `ddev launch /login`


### Moodle Configuration
When running the service via DDEV, the `config.php` file is located in the `public` dir. The documentation for this configuration can be found [here](https://docs.moodle.org/405/en/Configuration_file).