# Social Work Practice Development Programme (SWPDP) digital service

This repository houses the digital service for the Social Work Practice Development Programme (SWPDP), which is delivered using a number of applications:

- User management (`user-management`) provides a secure web-accessible frontend to allow organisations and users to be managed.
  - The `notification-service` provides access to [GOV.UK Notify](https://www.notifications.service.gov.uk) to support `user-management`. 
- Auth service (`auth-service`) provides an API for both authentication and data retrieval/persistence for `user-management`
  - The `auth-db` service provides a database instance to support `auth-service`
  - The `onelogin-simulator` provides a local [One Login](https://www.sign-in.service.gov.uk) simulator to support `auth-service`.
- Moodle service (`moodle-ddev`) provides a secure web-accessible frontend to provide access to learning materials and assessment tools.
  - The standalone [GOV.UK Moodle Theme](https://github.com/DFE-Digital/govuk-moodle-theme) adds [GDS](https://design-system.service.gov.uk/) compliance to Moodle.
  - The standlone [GOV.UK Moodle Assessment Activity](https://github.com/DFE-Digital/govuk-moodle-assessment-activity) adds data collection and workflow for an Assessment activity.

## Starting LOCALDEV

Once the **LOCALDEV one-time installation** has been run once, the following commands will bring up the service:

```bash
cd ~/swpdp/apps/auth-service/tools/auth-db
docker compose up -d
**auth-db running on localhost:5432

cd ~/swpdp/apps/auth-service
just onelogin-sim start
**onelogin-sim available at https://localhost:9010/onelogin/config

cd ~/swpdp/apps/auth-service
just watch-authz
**auth-service available at https://localhost:7236/.well-known/openid-configuration

cd ~/swpdp/apps/user-management/apps/notification-service/DfeSwwEcf.NotificationService
func start
**notification-service available at http://localhost:7071/api/health

cd ~/swpdp/apps/user-management/apps/frontend
dotnet watch run --launch-profile https
**user-management available at https://localhost:7244

cd ~/swpdp/apps/moodle-ddev
ddev start
**moodle-ddev available at https://moodle.ddev.site or https://moodle.ddev.site?noredirect=1
```

## LOCALDEV one-time installation

**This setup process assumes a LOCALDEV environment using [Ubuntu 24.04](https://ubuntu.com/blog/tag/ubuntu-24-04-lts)**

### Set up Ubuntu 24.04 on [Windows Subsystem for Linux (WSL)](https://learn.microsoft.com/en-us/windows/wsl/about)

```bash
[Windows PowerShell] Remove any previous WSL distributions and install a new one:
wsl --list --verbose
wsl --unregister Ubuntu-24.04
wsl --install Ubuntu-24.04
Create a default Unix user account: <LOCAL USERNAME AND PASSWORD>
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
```

Create `~/.ssh/config`:
```bash
Host github.com
AddKeysToAgent yes
IdentityFile ~/ssh-keys/github_swpdp
```

Add the SSH keypair to GitHub:
- Logged into GitHub, go to https://github.com/settings/ssh/new
- Title: 'GitHub SWPDP'
- Key type: Authentication Key
- Key: Contents of `~/ssh-keys/github_swpdp.pub`

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
1. Set up Docker's `apt` repository:
```bash
# Add Docker's official GPG key:
sudo apt update
sudo apt install ca-certificates curl
sudo install -m 0755 -d /etc/apt/keyrings
sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
sudo chmod a+r /etc/apt/keyrings/docker.asc

# Add the repository to Apt sources:
sudo tee /etc/apt/sources.list.d/docker.sources <<EOF
Types: deb
URIs: https://download.docker.com/linux/ubuntu
Suites: $(. /etc/os-release && echo "${UBUNTU_CODENAME:-$VERSION_CODENAME}")
Components: stable
Signed-By: /etc/apt/keyrings/docker.asc
EOF

sudo apt update
```
2. Install the Docker packages:
```bash
sudo apt install docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
```

3. Verify that the installation is successful by running the `hello-world` image:
```bash
sudo docker run hello-world
```

When the `hello-world` container runs, it prints a confirmation message and exits.

4. Add the current user to the docker group:
```bash
getent group docker
sudo usermod -aG docker $USER
getent group docker
```

5. Log out of the current SSH shell and log in again.

### Set up and run `auth-service`

#### 1. Dependency: `auth-db` Postgres service
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

#### 2: Set up and run `auth-service`

```bash
- Install dependencies from apps/auth-service/.tool-versions
cd ~/swpdp/apps/auth-service
mise install

Edit ~/.bashrc and add:
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

- Run `auth-service`
cd ~/swpdp/apps/auth-service
just watch-authz
```

**`auth-service` is now available at https://localhost:7236/.well-known/openid-configuration**

#### 3: Optional dependency: `onelogin-simulator` service
```bash
cd ~/swpdp/apps/auth-service
just onelogin-sim setup (Creates SSL certificate in ~/swpdp/apps/auth-service/tools/onelogin-simulator/certs)
just onelogin-sim start
``` 

**One Login simulator is now available at https://localhost:9010/onelogin/config**

### Set up and run `user-management`

#### 1. Dependency: `notification-service`

1. Generate a GOV.UK Notify API key:
- Go to https://www.notifications.service.gov.uk, register and log in
- From left-hand menu click **API integration**
- From top tabs, click **API keys**
- Click **Create an API key**
  - Name for this key: `<USERNAME>_localdev`
  - Type of key: **Team and guest list - limits who you can send to**

2. Install [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=linux%2Cisolated-process%2Cnode-v4%2Cpython-v2%2Chttp-trigger%2Ccontainer-apps&pivots=programming-language-csharp)

```bash
# Install the Microsoft package repository GPG key, to validate package integrity:
cd
curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg
sudo mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg
# Set up the APT source list before doing an APT update.
sudo sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-ubuntu-$(lsb_release -cs 2>/dev/null)-prod $(lsb_release -cs 2>/dev/null) main" > /etc/apt/sources.list.d/dotnetdev.list'
# Start the APT source update:
sudo apt-get update
# Install the Core Tools package:
sudo apt-get install azure-functions-core-tools-4
```

3. Edit `apps/user-management/apps/notification-service/DfeSwwEcf.NotificationService/local.settings.json`: add the `GovNotify__ApiKey`:
```bash
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "GovNotify__ApiKey": "<GOV.UK NOTIFY API KEY>"
  }
}
```

4. Run `notification-service`:
```bash
cd ~/swpdp/apps/user-management/apps/notification-service/DfeSwwEcf.NotificationService
func start
```

**`notification-service` is now available at http://localhost:7071/api/health**

#### 2. Set up and run `user-management`

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

## Add 'trust' for all SSL certificates to Windows and WSL

Remove any previously-trusted certificates:
- In Windows, run `certmgr.msc`
- Navigate to **Certificates - Current User** > **Trusted Root Certification Authorities** > **Certificates**
- Remove two `localhost` (expiring a year from now) and one `mkcert...` certificates

Locate `apps/auth-service/Dfe.Sww.Ecf/src/Dfe.Sww.Ecf.AuthorizeAccess/aspnet-dev-cert.pfx`
and double click it to start the **Certificate Import Wizard**.
- Store Location: **Current User**
- Click **Next** twice
- Password: **password123**
- Click **Next**
- Place all certificates in the following store: **Trusted Root Certification Authorities**
- Click **Next**, then **Finish**, then complete the process

Even though we've added the certificate to Windows Trusted Root Certification Authorities,
WSL/Linux has its own trust store. **WSL does not automatically use Windows root certificates**.

Add the certificate to WSL/Linux trust store:
```bash
sudo cp ~/swpdp/apps/auth-service/Dfe.Sww.Ecf/src/Dfe.Sww.Ecf.AuthorizeAccess/aspnet-dev-cert.crt /usr/local/share/ca-certificates/aspnet-dev-cert.crt
sudo update-ca-certificates
```
The above commands should report `1 added, 0 removed; done`.

At this stage, trust the other SSL certificates:

1. https://localhost:9010 is secured by a certificate generated by mkcert. We have to install the mkcert *root CA*, not the higher-level certificate.
   - Copy `~/.local/share/mkcert/rootCA.pem` to the Downloads directory and rename it `rootCA.pem.crt`
   - Double-click `rootCA.pem.crt` and **Install Certificate...**
     - Store Location: **Current User** and click **Next**.
     - Place all certificates in the following store: **Trusted Root Certification Authorities**.
     - Click **Next**, then **Finish**, then complete the process.
2. Go to https://localhost:7244 and export the certificate via **Not secure** > **Certificate details** > **Details** tab > **Export...**
   - Exports `localhost.crt`
   - Double-click the exported `localhost.crt` and **Install Certificate...**
     - Store Location: **Current User** and click **Next**.
     - Place all certificates in the following store: **Trusted Root Certification Authorities**.
     - Click **Next**, then **Finish**, then complete the process.

Verify the above by running `certmgr.msc` in Windows
- Navigate to **Certificates - Current User** > **Trusted Root Certification Authorities** > **Certificates**
- There should be two `localhost` (expiring a year from now) and one `mkcert...` (expiring 10 years from now) certificates.

## Set up and run `moodle-ddev`

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

Docker container needs to know about the outside world: create `apps/moodle-ddev/.ddev/docker-compose.override.yaml`
```bash
version: '3.6'

services:
  web:
    extra_hosts:
      - "host.docker.internal:host-gateway"
```

```bash
cd ~/swpdp/apps/moodle-ddev
mise install
just setup
Permission to beam up? n
```

The above commands:
- Runs Moodle's CLI installer.
- Moves the `install-scripts\setup_moodle_webservice.php` file into Moodle's public folder, enabling Moodle's web service.
- Downloads and installs the [GOV.UK Moodle Theme](https://github.com/DFE-Digital/govuk-moodle-theme) into `apps/moodle-ddev/public/theme`

**`moodle-dev` is now  available at https://moodle.ddev.site or https://moodle.ddev.site?noredirect=1**

ddev command for viewing useful configuration, including external ports:
```bash
cd ~/swpdp/apps/moodle-ddev; ddev describe
```

### Configure Moodle to use the GOV.UK SWPDP theme

In addition to the [GOV.UK Moodle Theme](https://github.com/DFE-Digital/govuk-moodle-theme), there is a SWPDP-specific version
of this theme, which needs to be copied in order to make it selectable:
```bash
cp -r ~/swpdp/apps/moodle-ddev/plugins/theme/govuk_swpdp ~/swpdp/apps/moodle-ddev/public/theme/.
```

- Log into https://moodle.ddev.site?noredirect=1 as **admin**.
- Click **Site administration** - Moodle prompts for the theme to be installed: click **Upgrade Moodle database now**.
- The **GOV.UK SWPDP** theme can now be selected at https://moodle.ddev.site/admin/themeselector.php

### Testing the SSO integration

To test the integration, ensure there is an account in the SWIP auth database that can authenticate successfully via GOV.UK OneLogin. Similarly, ensure there is a corresponding user account in the Moodle database.

To create this account in Moodle, navigate to `Site administration > Users > Accounts > Add a new user`. Set the `username` to the value of the email address stored for the SWIP user account. This will be used for account matching after authentication. Also select OpenID Connect for the authentication method of this account.

If needed, enable Moodle OIDC plugin debug manually in the user interface (Site Administration > Plugins > Authentication > OpenId Connect > Debugging section) or by using `moosh config-set debugmode 1 auth_oidc` with moosh installed and after running `ddev ssh`.

Manually creating the user account in Moodle which will be authenticating via SSO is a workaround until Moodle is integrated with the account management app and users are created programmatically.
