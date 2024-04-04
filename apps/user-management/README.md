# Social Worker Workforce - Early Career Framework

## Local Development

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### Setup

#### HTTPS

The applications explicitly run on HTTPS. For local development, you will need to generate a certificate and set it's
location via [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0).

To generate a certificate and configure user secrets, run the following from the project root directory:
```shell
# Generate a random certificate passkey
CERT_PASSKEY=$(cat /dev/urandom | LC_ALL=C tr -dc 'a-zA-Z0-9' | fold -w 50 | head -n 1)

# Generate a certificate and add it to your trusted keychain
# You will be prompted for your user password to trust the cert
CERT_NAME=dfe.sww.ecf.pfx
CERT_PATH=${HOME}/.aspnet/https/${CERT_NAME}
dotnet dev-certs https -ep ${CERT_PATH} -p ${CERT_PASSKEY} --trust

# Set the certificate path and passkey in project user secrets
# TODO: Remove this step and use `.env` for non-docker configuration also
dotnet user-secrets --id e812e454-6fd2-4bba-8173-9ea4d87f32b1 set "Kestrel:Certificates:Development:Path" ${CERT_PATH}
dotnet user-secrets --id e812e454-6fd2-4bba-8173-9ea4d87f32b1 set "Kestrel:Certificates:Development:Password" ${CERT_PASSKEY}
```
This will allow you to run the projects via the `dotnet` CLI or your IDE.

If you wish to run the project via [Docker Compose](https://docs.docker.com/compose/), you will also need to specify your certificate details in a `.env` file:
```shell
# Copy the `.env.example` template to `.env`
cp .env.example .env

# Update the certificate environment variables with the newly generated values
sed -i '' "/^DOCKER__KESTREL__CERTIFICATE_PASSWORD=/s/=.*/=${CERT_PASSKEY}/" .env
sed -i '' "/^DOCKER__KESTREL__CERTIFICATE_PATH=/s/=.*/=\/https\/${CERT_NAME}/" .env
```

### Running

#### Docker Compose

To run the project via the CLI from the project root directory, simply run `docker compose up -d`. 