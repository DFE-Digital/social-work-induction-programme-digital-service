# Social Work Practice Development Programme (SWPDP) digital service

This repository houses the digital service for the Social Work Practice Development Programme (SWPDP), which is delivered using a number of applications:

- User management (`user-management`) provides a secure web-accessible frontend to allow organisations and users to be managed.
- Auth service (`auth-service`) provides an API for both authentication and data retrieval/persistence for `user-management`
  - The `auth-db` tool provides a database instance to support `auth-service`
  - The `onelogin-simulator` tool provides a local [One Login](https://www.sign-in.service.gov.uk) simulator to support `auth-service`.
- Moodle service (`moodle-ddev`) provides a secure web-accessible frontend to provide access to learning materials and assessment tools.
  - The standalone [GOV.UK Moodle Theme](https://github.com/DFE-Digital/govuk-moodle-theme) adds [GDS](https://design-system.service.gov.uk/) compliance to Moodle.
  - The standlone [GOV.UK Moodle Assessment Activity](https://github.com/DFE-Digital/govuk-moodle-assessment-activity) adds data collection and workflow for an Assessment activity.

## Local development setup

**This setup process assumes a LOCALDEV environment using [Ubuntu 24.04](https://ubuntu.com/blog/tag/ubuntu-24-04-lts)**

### Set up Ubuntu 24.04 on [Windows Subsystem for Linux (WSL)](https://learn.microsoft.com/en-us/windows/wsl/about)

```bash
[Windows PowerShell] Remove any previous WSL distributions and install a new one:
wsl --list --verbose
wsl --unregister Ubuntu-24.04
wsl --install Ubuntu-24.04
Create a single local user
```

### Set up an SSH keypair to access GitHub
 ```bash
cd
mkdir ssh-keys
cd ssh-keys
ssh-keygen -t ed25519 -b 4096 -C "<DEVELOPER_EMAIL_ADDRESS>" -f github_swpdp
Enter passphrase (empty for no passphrase): <EMPTY: PRESS RETURN>

eval `ssh-agent -s`
ssh-add ~/ssh-keys/github_swpdp
mkdir ~/.ssh
vi ~/.ssh/config (new file) and add:
-----8<-----8<-----8<-----8<-----8<-----8<-----8<-----
Host github.com
AddKeysToAgent yes
IdentityFile ~/ssh-keys/github_swpdp
-----8<-----8<-----8<-----8<-----8<-----8<-----8<-----
```

Add the SSH keypair to GitHub:
- Logged into GitHub, go to https://github.com/settings/ssh/new
- Title: 'GitHub SWPDP'
- Key type: Authentication Key
- Key: Contents of ~/ssh-keys/github_swpdp.pub

### Check out the SWPDP GitHub repository

```bash
cd
mkdir swpdp
cd swpdp
git clone git@github.com:DFE-Digital/social-work-induction-programme-digital-service.git .
```

#### TODO: Add apps/auth-service/Dfe.Sww.Ecf/src/Dfe.Sww.Ecf.AuthorizeAccess/appsettings.Development.json

#### TODO: Add apps/user-management/apps/frontend/appsettings.Development.json

### Install the [mise](https://mise.jdx.dev/installing-mise.html) tool version manager
```bash
sudo apt update -y && sudo apt install -y gpg sudo wget curl
sudo install -dm 755 /etc/apt/keyrings
wget -qO - https://mise.jdx.dev/gpg-key.pub | gpg --dearmor | sudo tee /etc/apt/keyrings/mise-archive-keyring.gpg 1> /dev/null
echo "deb [signed-by=/etc/apt/keyrings/mise-archive-keyring.gpg arch=amd64] https://mise.jdx.dev/deb stable main" | sudo tee /etc/apt/sources.list.d/mise.list
sudo apt update
sudo apt install -y mise
```

### Install [docker](https://docs.docker.com/engine/install/ubuntu/)
```bash
sudo apt-get update
sudo apt-get install ca-certificates curl
sudo install -m 0755 -d /etc/apt/keyrings
sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
sudo chmod a+r /etc/apt/keyrings/docker.asc

echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu \
  $(. /etc/os-release && echo "${UBUNTU_CODENAME:-$VERSION_CODENAME}") stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
sudo apt-get update

sudo apt-get install docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
sudo docker run hello-world
```

When the `hello-world` container runs, it prints a confirmation message and exits.

Add the current user to the docker group:
```bash
getent group docker
sudo usermod -aG docker $USER
getent group docker
```

Log out of your SSH shell and log in again.

### Set up and run `auth-service`

#### `auth-db` Postgres service
```bash
cd ~/swpdp/apps/auth-service/tools/auth-db
docker compose up -d
```

**Postgres is now running on localhost:5432 (auth_user / auth_pass)**

<font color="orange">If Postgres is not running on localhost:5432, may need to update `apps/auth-service/tools/auth-db/compose.yml`</font>

<font color="orange">Change:</font>
```bash
      - pgdata:/var/lib/postgresql/data
```
<font color="orange">To:</font>
```bash
      - pgdata:/var/lib/postgresql/18/main
```
<font color="orange">Retry:</font> `docker compose up -d`

#### `onelogin-simulator` service
```bash
cd ~/swpdp/apps/auth-service
just onelogin-sim setup (Creates SSL certificate in ~/swpdp/apps/auth-service/tools/onelogin-simulator/certs)
just onelogin-sim start
``` 

**One Login simulator is now available at https://localhost:9010/onelogin/config**

#### Set up `auth-service`

```bash
- Install dependencies from apps/auth-service/.tool-versions
cd ~/swpdp/apps/auth-service
mise install

vi ~/.bashrc and add:
-----8<-----8<-----8<-----8<-----8<-----8<-----8<-----
eval "$(mise activate bash)"
-----8<-----8<-----8<-----8<-----8<-----8<-----8<-----
source ~/.bashrc

- Set up
cd ~/swpdp/apps/auth-service
just install-tools
just restore
just set-db-connection "Host=localhost;Username=auth_user;Password=auth_pass;Database=sww-ecf"
just set-test-db-connection "Host=localhost;Username=auth_user;Password=auth_pass;Database=sww-ecf_tests"
just build
just cli migrate-db

- Create SSL certificate
cd ~/swpdp/apps/auth-service/Dfe.Sww.Ecf/src/Dfe.Sww.Ecf.AuthorizeAccess
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout aspnet-dev-cert.key \
    -out aspnet-dev-cert.crt \
    -subj "/CN=localhost" \
    -addext "subjectAltName = DNS:localhost, DNS:host.docker.internal"
- This creates aspnet-dev-cert.crt and aspnet-dev-cert.key
openssl pkcs12 -export -out aspnet-dev-cert.pfx -inkey aspnet-dev-cert.key -in aspnet-dev-cert.crt -password pass:password123
- This creates aspnet-dev-cert.pfx

cd ~/swpdp/apps/auth-service
just watch-authz
```

**`auth-service` is now available at https://localhost:7236/.well-known/openid-configuration**

#### Set up `user-management`

```bash
cd ~/swpdp/apps/user-management
mise install
pnpm i
pnpm exec playwright install
pnpm exec playwright install-deps

cd ~/swpdp/apps/user-management/apps/frontend
dotnet watch run --launch-profile https
```

**`user-management` is now available at https://localhost:7244**

<font color="orange">At this stage, clicking through to **Sign in with GOV.UK One Login** gives an UntrustedRoot error</font>

#### Add SSL certificates to Windows and WSL

```bash
Remove any previously-trusted certificates:
- In Windows, run 'certmgr.msc'
- Navigate to Certificates - Current User > Trusted Root Certification Authorities > Certificates
- Remove 2 localhost (expiring a year from now) and 1 'mkcert...' certificates

Locate swpdp/apps/auth-service/Dfe.Sww.Ecf/src/Dfe.Sww.Ecf.AuthorizeAccess/aspnet-dev-cert.pfx
and double click it to start the Certificate Import Wizard.
- Store Location: Current User
- Click 'Next' twice
- Password: password123
- Place all certificates in the following store: Trusted Root Certification Authorities
- Complete the process

BUT... even though we've added the certificate to Windows Trusted Root Certification Authorities,
WSL/Linux has its own trust store. ***WSL does not automatically use Windows' root certificates***

Add the certificate to WSL/Linux trust store
sudo cp ~/swpdp/apps/auth-service/Dfe.Sww.Ecf/src/Dfe.Sww.Ecf.AuthorizeAccess/aspnet-dev-cert.crt /usr/local/share/ca-certificates/aspnet-dev-cert.crt
sudo update-ca-certificates
- Should report '1 added'

At this stage, trust the other SSL certificates

https://localhost:9010 is secured by a certificate generated by mkcert.
We have to install the mkcert *root CA*, not the higher-level certificate.
copy ~/.local/share/mkcert/rootCA.pem to the Downloads directory and rename it rootCA.pem.crt
Double-click rootCA.pem.crt and Install Certificate as Current User > Trusted Root Certification Authorities

Go to https://localhost:7244 and export the certificate via Not secure > Certificate details > Details > Export...
Exports localhost.crt
Double-click the exported .crt file and Install Certificate as Current User > Trusted Root Certification Authorities

Verify the above by running 'certmgr.msc' in Windows > Trusted Root Certification Authorities > Certificates
There should be 2 localhost (expiring a year from now) and 1 'mkcert...' (expiring 10 years from now)
```

#### Set up `moodle-ddev`

Install [DDEV](https://docs.ddev.com/en/stable/users/install/ddev-installation/#linux):

```bash
# Add DDEV’s GPG key to your keyring
sudo sh -c 'echo ""'
sudo apt-get update && sudo apt-get install -y curl
sudo install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://pkg.ddev.com/apt/gpg.key | gpg --dearmor | sudo tee /etc/apt/keyrings/ddev.gpg > /dev/null
sudo chmod a+r /etc/apt/keyrings/ddev.gpg
# Add DDEV releases to your package repository
sudo sh -c 'echo ""'
echo "deb [signed-by=/etc/apt/keyrings/ddev.gpg] https://pkg.ddev.com/apt/ * *" | sudo tee /etc/apt/sources.list.d/ddev.list >/dev/null
# Update package information and install DDEV
sudo sh -c 'echo ""'
sudo apt-get update && sudo apt-get install -y ddev
# One-time initialization of mkcert: THIS HAS ALREADY BEEN DONE
# mkcert -install
```

Docker container needs to know about the outside world: create apps/moodle-ddev/.ddev/docker-compose.override.yaml
```bash
-----8<-----8<-----8<-----8<-----8<-----8<-----8<-----
version: '3.6'

services:
  web:
    extra_hosts:
      - "host.docker.internal:host-gateway"
-----8<-----8<-----8<-----8<-----8<-----8<-----8<-----
```

```bash
cd ~/swpdp/apps/moodle-ddev
mise install
just setup
Permission to beam up? n
```

**`moodle-dev` is now  available at https://moodle.ddev.site or https://moodle.ddev.site?noredirect=1**

ddev command for viewing useful configuration, including external ports:
```bash
cd ~/swpdp/apps/moodle-ddev; ddev describe
```






# Social Work Induction Programme (SWIP) digital service

This repository houses the core digital service for the Social Work Induction programme in children's social care. The digital service is based on [Moodle LMS](https://moodle.org).

## Local development setup

The `apps/moodle-ddev` project provides a consistent local environment for running Moodle. Custom DDEV commands have been created to automate common tasks such as installing Moodle and setting up the govuk theme.

The primary custom command, install-moodle, bundles all installation steps into a single command. It performs the following tasks:

- Runs Moodle’s CLI installer.
- Downloads and installs the govuk theme into the correct directory.
- Updates config.php to set the default theme to govuk.
- Purges Moodle caches so that changes take effect immediately.

#### Windows setup
You need to clone this repository into your user directory in the WSL directory. This is usually found in `\\wsl.localhost\Ubuntu\home\{USERNAME}`.

---

### DDEV installation and setup
Follow the instructions at [DDEV](https://ddev.readthedocs.io/en/stable/users/install/ddev-installation) for your operating system.

#### Quick project setup
Once you have DDev installed on your machine, you can either run through the setup process step-by-step, or utilise the `just setup` script to utilise the default settings and get up and running quickly.

> Note: You will need [just](https://just.systems/man/en/) installed on your system. A `.tool-versions` file exists in the project so you can use a tool like [asdf](https://asdf-vm.com/) to install `just`.

```bash
just setup
```

This will install Moodle, add the latest GovUK Moodle theme, setup a Moodle web service, and add the [OIDC plugin](https://moodle.org/plugins/auth_oidc); feel free check the `justfile` to see exactly what this script does.

---

#### Manual project setup
If you would like to setup your project step-by-step and specify different parameters, you can follow these steps:

```bash
# Start your DDEV environment
ddev start

# Use Composer to create the new Moodle installation
ddev composer create-project moodle/moodle . "v4.5.5" # You can specify a different moodle version here

# Use the custom DDEV commands to perform the full installation
ddev install-moodle
# Run the DDev command to install the GovUK Moodle theme
# You can specify a different version of the DfE-Digital/govuk-moodle-theme package or omit the version to use the latest
ddev install-theme "v0.1.0"

# Finally, launch your Moodle site in your default web browser
ddev launch /login
```

##### Create a Moodle a web service
[Web services](https://docs.moodle.org/405/en/Web_services) enable other systems to login to Moodle and perform operations. We have created scripts to automate this process.

- Run `ddev move-setup-webservice`
  - This moves the `install-scripts\setup_moodle_webservice.php` file into Moodle's public folder, making it available to ddev.
  - **Important** - You will need to run this command everytime before running the below command. Any updates to the `setup_moodle_webservice.php` are commited to the `install-scripts\setup_moodle_webservice.php` version of this file. We need to run this script to move the newest version of the file into Moodle's `public` directory.

- Run `ddev setup-ws {webservice_user} {webservice_password} {webservice_email} {webservice_servicename}`
  - This will run the `setup-ws` ddev script, which references the above `setup_moodle_webservice.php` file. It has four optional inputs, e.g. `ddev setup-ws test password123! wsuser@example.com SwipService`. You can also run this script without any inputs and it will use default parameters by running `ddev setup-ws`.

##### Single sign-on (SSO) configuration
Moodle integrates with the SWIP authentication service for users to log in via single sign-on and GOV.UK One Login. The [Moodle OpenID Connect (OIDC)](https://moodle.org/plugins/auth_oidc) plugin is used.

###### Install and configure OIDC plugin

The following steps are required to install and configure the plugin. These should be carried out after `ddev start`. If this is done as part of initial moodle setup on your local environment, then follow this guide after you complete the setup in [Running the service](#running-the-service).

1. Run `ddev install-moosh`

The command will download and install the [moosh plugin](https://moosh-online.com/) which exposes Moodle functionality to the command line. This will be used to configure the OIDC plugin.

2. Run `ddev install-oidc-plugin`

The command will download, install and configure the OIDC plugin. It will also purge all caches, which is needed for the config changes to take effect.
This script can also take an OIDC plugin release URL as a parameter, which will be used instead of the default release URL.
If the OIDC plugin is already installed, the script will exit.

Once the OIDC plugin is installed and configured, the login page will be bypassed by default. To access Moodle as admin, use the query string parameter `noredirect=1`, e.g. https://moodle.ddev.site/?noredirect=1.

---

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
