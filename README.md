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
After configuring your project with DDEV, follow these steps from this project’s `apps\moodle-ddev` directory:

Configure DDEV with both the composer root and document root set to public:

- `ddev config --composer-root=public --docroot=public --webserver-type=apache-fpm --database=postgres:14 --project-name=moodle`

Start your DDEV environment:

- `ddev start`

Use Composer to create the new Moodle installation:

- `ddev composer create moodle/moodle`

Use the custom DDEV commands to perform the full installation, theme import, configuration update, and cache purge:

- `ddev install-moodle`
- `ddev install-theme`

Finally, launch your Moodle site in your default web browser:

- `ddev launch /login`

---
### Create a Moodle a web service
[Web services](https://docs.moodle.org/405/en/Web_services) enable other systems to login to Moodle and perform operations. We have created scripts to automate this process.

- Run `ddev move-setup-webservice`
  - This moves the `install-scripts\setup_webservice.php` file into Moodle's public folder, making it available to ddev.
  - **Important** - You will need to run this command everytime before running the below command. Any updates to the `setup_webservice.php` are commited to the `install-scripts\setup_webservice.php` version of this file. We need to run this script to move the newest version of the file into Moodle's `public` directory.

- Run `ddev setup-ws {webservice_user} {webservice_password} {webservice_email} {webservice_servicename}`
  - This will run the `setup-ws` ddev script, which references the above `setup_webservice.php` file. It has four optional inputs, e.g. `ddev setup-ws test password123! wsuser@example.com SwipService`. You can also run this script without any inputs and it will use default parameters by running `ddev setup-ws`.

---
### Single sign-on (SSO) configuration
Moodle integrates with the SWIP authentication service for users to log in via single sign-on and GOV.UK One Login. The [Moodle OpenID Connect (OIDC)](https://moodle.org/plugins/auth_oidc) plugin is used.

#### Install and configure OIDC plugin

The following steps are required to install and configure the plugin. These should be carried out after `ddev start`. If this is done as part of initial moodle setup on your local environment, then follow this guide after you complete the setup in [Running the service](#running-the-service).

1. Run `ddev install-moosh`

The command will download and install the [moosh plugin](https://moosh-online.com/) which exposes Moodle functionality to the command line. This will be used to configure the OIDC plugin.

2. Run `ddev install-oidc-plugin`

The command will download, install and configure the OIDC plugin. It will also purge all caches, which is needed for the config changes to take effect.
This script can also take an OIDC plugin release URL as a parameter, which will be used instead of the default release URL.
If the OIDC plugin is already installed, the script will exit.

Once the OIDC plugin is installed and configured, the login page will be bypassed by default. To access Moodle as admin, use the query string parameter `noredirect=1`, e.g. https://moodle.ddev.site/?noredirect=1.

#### Generate and install SSL certificates

For the Moodle instance running in a container to connect with the SWIP authentication service running on localhost, custom SSL certificates are needed. These steps are carried out once, as part of local Moodle setup.
Navigate to the `AuthorizeAccess` project in the `csc-social-work-ecf-digital-auth` repository on your machine and open Git bash in that location. From Git bash, generate a self-signed SSL certificate using openssl:

```sh
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout aspnet-dev-cert.key \
    -out aspnet-dev-cert.crt \
    -subj "//CN=localhost" \
    -addext "subjectAltName = DNS:localhost, DNS:host.docker.internal"
```

Then generate a `pfx` file based on the key and the certificate  and set a password (the password must match the app settings in the SWIP auth service):
`openssl pkcs12 -export -out aspnet-dev-cert.pfx -inkey aspnet-dev-cert.key -in aspnet-dev-cert.crt -password pass:PASSWORD`

You will need administrative privileges on your local machine to install the certificate.

If using Windows, double-click on the generated `aspnet-dev-cert.pfx` file and select "Local machine" for the "Store Location". Continue with the setup until you have to select the certificate store. Select to use Trusted Root Certification Authorities and complete the installation process.

If using a Mac, run:
`sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain aspnet-dev-cert.crt`

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

Next:
- create a `custom_certs` folder under `.ddev` where your Moodle code is stored (e.g. `\\wsl.localhost\Ubuntu\home\{username}\repos\social-work-induction-programme-digital-service\apps\moodle-ddev\.ddev`)
- copy the `aspnet-dev-cert.key` and `aspnet-dev-cert.crt` files to the `custom_certs` directory
- rename the files to the name of the ddev project (`moodle.key` and `moodle.crt`)
- open the moodle certificate file and select "Install Certificate..."; follow the same steps as above to select the Trusted Root certificate store and install the certificate

#### Testing the SSO integration
To test the integration, ensure there is an account in the SWIP auth database that can authenticate successfully via GOV.UK OneLogin. Similarly, ensure there is a corresponding user account in the Moodle database.

To create this account in Moodle, navigate to `Site administration > Users > Accounts > Add a new user`. Set the `username` to the value of the email address stored for the SWIP user account. This will be used for account matching after authentication. Also select OpenID Connect for the authentication method of this account.

If needed, enable Moodle OIDC plugin debug manually in the user interface (Site Administration > Plugins > Authentication > OpenId Connect > Debugging section) or by using `moosh config-set debugmode 1 auth_oidc` with moosh installed and after running `ddev ssh`.

Manually creating the user account in Moodle which will be authenticating via SSO is a workaround until Moodle is integrated with the account management app and users are created programmatically.

### Moodle configuration
When running the service via DDEV, the `config.php` file is located in the `public` dir. The documentation for this configuration can be found [here](https://docs.moodle.org/405/en/Configuration_file).

### Moodle tests
For documentation on automated testing in Moodle and local dev setup, please see the dedicated files in the `docs\moodle-docs` folder.
