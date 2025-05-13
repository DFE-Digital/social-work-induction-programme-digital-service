##############################################################
# Stage 1: Basic Moodle install - requires local postgres db #
##############################################################

FROM moodlehq/moodle-php-apache:8.3-bookworm AS base-moodle-install-builder

ARG MOODLE_BRANCH_VERSION

WORKDIR /var/www

# Clone the Moodle source code.
RUN git clone -b MOODLE_${MOODLE_BRANCH_VERSION}_STABLE --single-branch --depth=1 git://git.moodle.org/moodle.git

# Prune folders and files beginning with '.' as well as other non-essential files
RUN find ./moodle -mindepth 1 -type d -name '.*' -exec rm -rf {} + && \
    find ./moodle -type f \( -name ".*" -o -name "*.md" -o -name "*.dist" -o -name "*.lock" \) -delete && \
    rm -f ./moodle/*.json && \
    rm -f ./moodle/*.js && \
    rm -f ./moodle/*.txt && \
    rm -f ./moodle/config-dist.php

RUN echo "Starting Moodle CLI installation..." \
    && cd moodle \
    && php admin/cli/install.php \
    --non-interactive \
    --agree-license \
    --wwwroot="http://localhost/" \
    --dbtype=pgsql \
    --dbhost=localhost \
    --dbname=moodle \
    --dbuser=postgres \
    --dbpass=postgres \
    --fullname="Social Work Induction Programme" \
    --shortname=SWIP \
    --adminpass=password \
    --adminemail=admin@example.com \
    && echo "Moodle installation complete."

######################################################
# Stage 2: Fully customised Moodle app with local DB #
######################################################

FROM moodlehq/moodle-php-apache:8.3-bookworm AS full-moodle-builder

ARG MOODLE_GOV_UK_THEME_VERSION

# The theme script below runs the Moodle upgrade script from public/...
WORKDIR /var/www/html

# Copy the pruned / installed Moodle source
COPY --from=base-moodle-install-builder --chown=www-data:www-data /var/www/moodle/ ./public/

# Copy the moodledata directory created during the install
COPY --from=base-moodle-install-builder --chown=www-data:www-data /var/www/moodledata/ /var/www/moodledata/

# Install the custom GOVUK theme
COPY install-theme install-theme
RUN chmod +x install-theme \
    && ./install-theme ${MOODLE_GOV_UK_THEME_VERSION}

###############################################################
# Stage 3: Fully customised Moodle app prepped for Azure DB   #
###############################################################

FROM moodlehq/moodle-php-apache:8.3-bookworm AS moodle-app

ARG AZURE_KUDU_SSH_USER
ARG FULL_TAG

WORKDIR /var/www/html/public

# Mandatory tools for remote SSH
RUN apt-get update && apt-get install -y --no-install-recommends \
    openssh-server \
    postgresql-client \
    cron \
    && rm -rf /var/lib/apt/lists/*

# Support kudu tools SSH for remote debug
COPY sshd_config /etc/ssh/sshd_config
RUN --mount=type=secret,id=AZURE_KUDU_SSH_PASSWORD \
    export AZURE_KUDU_SSH_PASSWORD=$(cat /run/secrets/AZURE_KUDU_SSH_PASSWORD) \
    && echo "$AZURE_KUDU_SSH_USER:$AZURE_KUDU_SSH_PASSWORD" | chpasswd \
    && > /etc/motd \
    && mkdir -p /var/run/sshd \
    && chmod 0755 /var/run/sshd \
    && cd /etc/ssh/ \
    && ssh-keygen -A

# Copy the pruned Moodle source / downloaded theme / data
COPY --from=full-moodle-builder --chown=www-data:www-data /var/www/ /var/www

# Copy custom config.php into the correct location
COPY config.php ./config.php

# Convenient way of checking / ensuring tag version
RUN echo "$FULL_TAG" > version.txt

# Re-enable if DB connectivity tracing is required
#COPY dmllib.trace.php ./lib/dmllib.php

# Needs to run as root for various standard mount point permissions in Azure
# Apache is configured to run as www-data
USER root

# The maintain moodle script allows us to upgrade the moodle database at deployment time
RUN mkdir -p /app
COPY entry-point.sh /app/entry-point.sh
COPY maintain-moodle.sh /app/maintain-moodle.sh
COPY save-env.sh /app/save-env.sh
COPY restore-env.sh /app/restore-env.sh
RUN chmod +x /app/entry-point.sh /app/maintain-moodle.sh /app/save-env.sh /app/restore-env.sh

# Default config is full-blown moodle instance - if not the case, the entry point script
# will override this with the cron config
COPY apache-config-moodle.conf /etc/apache2/sites-available/000-default.conf
COPY apache-config-cron.conf /app/apache-config-cron.conf
COPY moodle-cron /app/moodle-cron

# Give the cron job some html to serve - the cron job service is still an app service
# and needs to serve something
RUN mkdir -p /var/www/html/cron
COPY index.html /var/www/html/cron/index.html
RUN chown -R www-data:www-data /var/www/html/cron

CMD ["/app/entry-point.sh"]

EXPOSE 80
