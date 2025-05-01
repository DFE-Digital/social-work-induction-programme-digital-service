# #!/bin/bash
# set -euo pipefail

# docker build \
#     --build-arg AZURE_KUDU_SSH_USER=root --build-arg MOODLE_BRANCH_VERSION=405 \
#     --secret id=AZURE_KUDU_SSH_PASSWORD,env=AZURE_KUDU_SSH_PASSWORD \
#     --build-arg FULL_TAG=wa-moodle:405-20250429.8c7c6e2.dev \
#     -f "./Dockerfile" \
#     --network=host \
#     --target moodle-app \
#     -t "wa-moodle:405-20250429.8c7c6e2.dev" .

# Assumes a tunnelled connection from localhost to DB server has already been established
chmod +x ./maintain-moodle.sh

MOODLE_ADMIN_EMAIL=admin@email.com
MOODLE_ADMIN_USER=admin
MOODLE_ADMIN_PASSWORD=blah
MOODLE_SITE_SHORTNAME=SWIP
MOODLE_SITE_FULLNAME="Social Work Induction Programme"
MOODLE_DB_TYPE=pgsql
MOODLE_DOCKER_WEB_HOST=s205d01-wa-moodle-primary.azurewebsites.net
POSTGRES_DB=moodle
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
MOODLE_DB_PREFIX=mdl_

docker run --rm \
    --network=host \
    -v ./maintain-moodle.sh:/var/www/html/maintain-moodle.sh:ro \
    -e "MOODLE_ADMIN_EMAIL=$MOODLE_ADMIN_EMAIL" \
    -e "MOODLE_ADMIN_USER=$MOODLE_ADMIN_USER" \
    -e "MOODLE_ADMIN_PASSWORD=$MOODLE_ADMIN_PASSWORD" \
    -e "MOODLE_SITE_SHORTNAME=$MOODLE_SITE_SHORTNAME" \
    -e "MOODLE_SITE_FULLNAME=$MOODLE_SITE_FULLNAME" \
    -e "MOODLE_DB_TYPE=$MOODLE_DB_TYPE" \
    -e "MOODLE_DB_HOST=127.0.0.1" \
    -e "MOODLE_DOCKER_WEB_HOST=$MOODLE_DOCKER_WEB_HOST" \
    -e "POSTGRES_DB=$POSTGRES_DB" \
    -e "POSTGRES_USER=$POSTGRES_USER" \
    -e "POSTGRES_PASSWORD=$POSTGRES_PASSWORD" \
    -e "MOODLE_DB_PREFIX=$MOODLE_DB_PREFIX" \
    wa-moodle:405-20250429.8c7c6e2.dev \
    bash -euo pipefail -c /var/www/html/maintain-moodle.sh