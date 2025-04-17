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

# Install tools needed for cloning, downloading, and unzipping.
USER root

# The theme script below runs the Moodle upgrade script from public/...
WORKDIR /var/www/html

# Copy the pruned / installed Moodle source
COPY --from=base-moodle-install-builder --chown=www-data:www-data /var/www/moodle/ ./public/

# Copy the moodledata directory created during the install
COPY --from=base-moodle-install-builder --chown=www-data:www-data /var/www/moodledata/ /var/www/moodledata/

# Install the custom GOVUK theme
COPY install-theme install-theme
RUN chmod +x install-theme \
    && ./install-theme

###############################################################
# Stage 3: Fully customised Moodle app prepped for Azure DB   #
###############################################################

FROM moodlehq/moodle-php-apache:8.3-bookworm AS moodle-app

WORKDIR /var/www/html/public

USER www-data

# Copy the pruned Moodle source / downloaded theme / data
COPY --from=full-moodle-builder --chown=www-data:www-data /var/www/ /var/www

# Copy custom config.php into the correct location
COPY config.php ./config.php
# Re-enable if DB connectivity tracing is required
#COPY dmllib.trace.php ./lib/dmllib.php

# Needs to run as root for various standard mount point permissions in Azure
# Apache is configured to run as www-data
USER root

COPY apache-config-moodle.conf /etc/apache2/sites-available/000-default.conf

RUN mkdir -p /app
COPY entry-point-moodle.sh /app/entry-point.sh
RUN chmod +x /app/entry-point.sh

CMD ["/app/entry-point.sh"]

EXPOSE 80

#######################################################################################
# Stage 1.5: Moodle installer app for Azure. Derived from fully installed base Moodle #
#            app. Will install additional themes and plugins. We need to do this      #
#            because those themes and plug-ins may also update the database and we    #
#            need to keep the Azure db in sync.                                       #
#######################################################################################

FROM moodlehq/moodle-php-apache:8.3-bookworm AS install-app

ARG AZURE_KUDU_SSH_USER

USER root

WORKDIR /var/www/html/public

# Copy the pruned / installed Moodle source
COPY --from=base-moodle-install-builder --chown=www-data:www-data /var/www/moodle/ .

# Copy the moodledata directory created during the install
COPY --from=base-moodle-install-builder --chown=www-data:www-data /var/www/moodledata/ /var/www/moodledata/

# Copy the custom GOVUK theme - but don't install - this will be done at install time
# when the Azure DB is accessible
COPY install-theme install-theme
RUN chmod +x install-theme \
    && ./install-theme default --copy-only \
    && chown -R www-data:www-data /var/www/html/public/theme \
    && rm install-theme

USER www-data

# Copy custom config.php into the correct location
COPY config.php ./config.php
# Re-enable if DB connectivity tracing is required
#COPY dmllib.trace.php ./lib/dmllib.php

COPY index.html /var/www/html/public/index.html

USER root

# Mandatory tools for installation + remote SSH
RUN apt-get update && apt-get install -y --no-install-recommends \
    postgresql-client \
    openssh-server \
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
        
# Helpful tools for remote debug via SSH
RUN apt-get update && apt-get install -y --no-install-recommends \
    iputils-ping \
    dnsutils \
    net-tools \
    traceroute \
    && rm -rf /var/lib/apt/lists/*

# Custom installation entry point - will run the entire install process
COPY entry-point-install.sh /app/entry-point.sh
RUN chmod +x /app/entry-point.sh
    
# Need to be root to update the apache config file and for Azure
COPY apache-config-install.conf /etc/apache2/sites-available/000-default.conf

CMD ["/app/entry-point.sh"]

EXPOSE 80
