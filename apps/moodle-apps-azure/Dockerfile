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
ARG MOODLE_OIDC_PLUGIN_RELEASE_URL
ARG PHP_COMPOSER_VERSION=2.6.5

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

# PHP composer is a prerequisite for installing Moosh
RUN curl -sS https://getcomposer.org/installer -o composer-setup.php \
    && php composer-setup.php --version=${PHP_COMPOSER_VERSION} --install-dir=/usr/local/bin --filename=composer \
    && rm composer-setup.php \
    && composer --version    

# Moosh is required to run the install-oidc-plugin script
COPY install-moosh install-moosh
RUN chmod +x install-moosh \
    && ./install-moosh \
    && rm install-moosh

# Required to run the install-oidc-plugin script
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    wget \
    unzip

RUN mkdir -p /app
COPY install-oidc-plugin /app/install-oidc-plugin
COPY setup_moodle_webservice.php /app/setup_moodle_webservice.php
RUN chmod +x /app/install-oidc-plugin \
    && cd public \
    && /app/install-oidc-plugin check,copy,upgrade \
    ${MOODLE_OIDC_PLUGIN_RELEASE_URL}

RUN rm install-*

###############################################################
# Stage 3: Fully customised Moodle app prepped for Azure DB   #
###############################################################

# Why 3 stages? We're only interested in the outputs of all of this configuration / install
# work. It keeps a clean boundary between the requirements of the final image and the
# actual and potential requirements of the intermediate stages. Even if we do have to 
# repeat some installation work for PHP composer / Moosh.

FROM moodlehq/moodle-php-apache:8.3-bookworm AS moodle-app

ARG AZURE_KUDU_SSH_USER
ARG FULL_TAG
ARG PHP_COMPOSER_VERSION=2.6.5

WORKDIR /var/www/html/public

# Mandatory tools for remote SSH + general mgmt and az for file sync
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    openssh-server \
    postgresql-client \
    apache2-utils \
    rsync \
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

# PHP composer is a prerequisite for installing Moosh
RUN curl -sS https://getcomposer.org/installer -o composer-setup.php \
    && php composer-setup.php --version=${PHP_COMPOSER_VERSION} --install-dir=/usr/local/bin --filename=composer \
    && rm composer-setup.php \
    && composer --version

# Moosh is required to run the install-oidc-plugin script at deployment time
COPY install-moosh install-moosh
RUN chmod +x install-moosh \
    && ./install-moosh \
    && rm install-moosh

COPY --from=full-moodle-builder /app/ /app
COPY entry-point.sh /app/entry-point.sh
# The maintain moodle script allows us to upgrade the moodle database at deployment time
COPY maintain-moodle.sh /app/maintain-moodle.sh
COPY save-env.sh /app/save-env.sh
COPY restore-env.sh /app/restore-env.sh
RUN chmod +x /app/*.sh

# Default config is full-blown moodle instance - if not the case, the entry point script
# will override this with the cron config
COPY apache-security.conf /etc/apache2/conf-enabled/security.conf
COPY apache-config-moodle.conf /etc/apache2/sites-available/000-default.conf
COPY apache-config-cron.conf /app/apache-config-cron.conf
RUN a2enmod headers

# Give the cron job some html to serve - the cron job service is still an app service
# and needs to serve something
COPY --chown=www-data:www-data index.html /var/www/html/public/index.html

CMD ["/app/entry-point.sh"]

EXPOSE 80
