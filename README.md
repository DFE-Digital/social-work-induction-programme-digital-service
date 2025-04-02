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
### Windows setup
You need to clone this repository into your user directory in the WSL directory. This is usually found in `\\wsl.localhost\Ubuntu\home\{USERNAME}`. 

### DDEV installation
Follow the instructions at [DDEV](https://ddev.readthedocs.io/en/stable/users/install/ddev-installation) for your operating system.

---
### Running the service
After configuring your project with DDEV, follow these steps from this project’s `moodle-app` directory:

Configure DDEV with both the composer root and document root set to public:

- `ddev config --composer-root=public --docroot=public --webserver-type=apache-fpm --database=postgres:14 --project-name=moodle`

Start your DDEV environment:

- `ddev start`

Use Composer to create a new Moodle installation:

- `ddev composer create moodle/moodle`

Use the custom DDEV commands to perform the full installation, theme import, configuration update, and cache purge:

- `ddev install-moodle`
- `ddev install-theme`

Finally, launch your Moodle site in your default web browser:

- `ddev launch /login`


### Moodle configuration
When running the service via DDEV, the `config.php` file is located in the `public` dir. The documentation for this configuration can be found [here](https://docs.moodle.org/405/en/Configuration_file).

### Moodle tests
For documentation on automated testing in Moodle and local dev setup, please see the dedicated files in the `moodle-docs` folder.

### Single sign-on (SSO) configuration
Moodle integrates with the SWIP authentication service for users to log in via single sign-on and GOV.UK One Login. The [Moodle OpenID Connect (OIDC)](https://moodle.org/plugins/auth_oidc) plugin is used. 

#### Install and configure OIDC plugin

The following steps are required to install and configure the plugin: 

1. Set script execution permissions by running `ddev set-permissions`

2. Run `ddev ssh`

3. Run `/var/www/html/scripts/install-oidc-plugin`
The script will download and install the [moosh plugin](https://moodle.org/plugins/view.php?id=522) that exposes Moodle functionality to the command line, as well download, install and configure the OIDC plugin.
This script can also take an OIDC plugin release URL as a parameter, which will be used instead of the default release URL.
If the OIDC plugin is already installed, the script will exit.

Once the OIDC plugin is installed and configured, the login page will be bypassed by default. To access Moodle as admin use the URL https://moodle.ddev.site/?noredirect=1.

#### Generate and install SSL certificates

For the Moodle instance running in a container to connect with the SWIP authentication service running on localhost, custom SSL certificates are needed. These steps are carried out once, as part of local Moodle setup.
Navigate to the `AuthorizeAccess` project in the `csc-social-work-ecf-digital-auth` repository on your machine and open Git bash in that location. From Git bash, generate a self-signed SSL certificate using openssl:

`openssl req -x509 -nodes -days 365 -newkey rsa:2048 \`
`    -keyout aspnet-dev-cert.key \`
`    -out aspnet-dev-cert.crt \`
`    -subj "//CN=localhost" \`
`    -addext "subjectAltName = DNS:localhost, DNS:host.docker.internal"`

Then generate a `pfx` file based on the key and the certificate  and set a password (the password must match the app settings in the SWIP auth service):
`openssl pkcs12 -export -out aspnet-dev-cert.pfx -inkey aspnet-dev-cert.key -in aspnet-dev-cert.crt -password pass:PASSWORD`

If using Windows, double-click on the generated `aspnet-dev-cert.pfx` file and select Current User for the Store Location. Continue with the setup until you have to select the certificate store. Select to use Trusted Root Certification Authorities and complete the installation process. 

If using a Mac, run:
`sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain aspnet-dev-cert.crt`
 apparently
Or, you can manually add the certificate via Keychain Access:
- Open Keychain Access
- Press Cmd + Space, type "Keychain Access", and hit Enter.
- Drag and Drop aspnet-dev-cert.crt into the "System" Keychain
- Make sure you’re in "System", not "login" or "Local Items".
- Trust the Certificate
- Find your certificate (it should have CN=localhost).
- Right-click → Get Info.
- Expand "Trust" and set "Always Trust".
- Close the window and enter your password to confirm.

You will need administrative privileges on your local machine to do this.

Next:
- create a `custom_certs` folder under `.ddev` where your Moodle code is stored (e.g. `\\wsl.localhost\Ubuntu\home\{username}\repos\social-work-induction-programme-digital-service\moodle-app\.ddev`) 
- copy the `aspnet-dev-cert.key` and `aspnet-dev-cert.crt` files to the `custom-certs` directory
- rename the files to the name of the ddev project (`moodle.key` and `moodle.crt`)
- open the moodle certificate file and select "Install Certificate..."; follow the same steps as above to select the Trusted Root certificate store and install the certificate

#### Testing the SSO integration
To test the integration, ensure there is an account in the SWIP auth database that can authenticate successfully via GOV.UK OneLogin. Similarly, ensure there is a corresponding user account in the Moodle database. 
To create this account in Moodle, navigate to `Site administration > Users > Accounts > Add a new user`. Set the `username` to the value of the email address stored for the SWIP user account. This will be used for account matching after authentication. Also select OpenID Connect for the authentication method of this account.
Manually creating the user account in Moodle which will be authenticating via SSO is a workaround until Moodle is integrated with the account management app and users are created programmatically. 