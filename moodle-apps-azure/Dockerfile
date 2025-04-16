##############################################################
# Stage 1: Basic Moodle install - requires local postgres db #
##############################################################

FROM moodlehq/moodle-php-apache:8.3-bookworm AS base-moodle-install-builder

ARG MOODLE_BRANCH_VERSION

WORKDIR /build

# Clone the Moodle source code.
RUN git clone -b MOODLE_${MOODLE_BRANCH_VERSION}_STABLE --single-branch --depth=1 git://git.moodle.org/moodle.git

# Prune folders and files beginning with '.' as well as other non-essential files
RUN find ./moodle -mindepth 1 -type d -name '.*' -exec rm -rf {} + && \
    find ./moodle -type f \( -name ".*" -o -name "*.js" -o -name "*.txt" -o -name -o -name "*.md" -o -name "*.dist" -o -name "*.lock" \) -delete && \
    rm -f ./moodle/*.json && \
    rm -f ./moodle/config-dist.php

RUN echo "Starting Moodle CLI installation..." \
    && php admin/cli/install.php \
    --non-interactive \
    --agree-license \
    --wwwroot="$DDEV_PRIMARY_URL" \
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

#####################################################
# Stage 2: Fully installed Moodle app with local DB #
#####################################################

FROM moodlehq/moodle-php-apache:8.3-bookworm AS full-moodle-builder

# Install tools needed for cloning, downloading, and unzipping.
USER root
RUN apt-get update && apt-get install -y --no-install-recommends \
      git \
      curl \
      unzip

WORKDIR /var/www/html/public

# Copy the pruned / installed Moodle source
COPY --from=base-moodle-install-builder --chown=www-data:www-data /build/moodle/ ./

# Install the custom GOVUK theme
COPY install-theme install-theme
RUN chmod +x install-theme \
    && ./install-theme

###############################################################
# Stage 3: Fully installed Moodle app customised for Azure DB #
###############################################################

FROM moodlehq/moodle-php-apache:8.3-bookworm AS moodle-app

WORKDIR /var/www/html/public

# Copy the pruned Moodle source / downloaded theme
COPY --from=base-moodle-install-builder --chown=www-data:www-data /build/moodle/ ./

# Copy custom config.php into the correct location
COPY config.php ./config.php
# Re-enable if DB connectivity tracing is required
#COPY dmllib.trace.php ./lib/dmllib.php

# For Apache: Update DocumentRoot to point to /var/www/html/public
ENV APACHE_CONF="/etc/apache2/sites-available/000-default.conf"
RUN if grep -q "^DocumentRoot " "$APACHE_CONF"; then \
        sed -i 's|^DocumentRoot .*|DocumentRoot /var/www/html/public|' "$APACHE_CONF"; \
    else \
        echo "DocumentRoot /var/www/html/public" >> "$APACHE_CONF"; \
    fi

RUN cat "$APACHE_CONF"

RUN mkdir -p /app
COPY entry-point-moodle.sh /app/entry-point.sh
RUN chmod +x /app/entry-point.sh

# Needs to run as root for various standard mount point permissions in Azure
# Apache is configured to run as www-data
USER root

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

# Install tools needed for cloning, downloading, and unzipping
USER root
RUN apt-get update && apt-get install -y --no-install-recommends \
      git \
      curl \
      unzip

WORKDIR /var/www/html/public

# Copy the pruned / installed Moodle source
COPY --from=base-moodle-install-builder --chown=www-data:www-data /build/moodle/ ./

# Copy custom config.php into the correct location
COPY config.php ./config.php
# Re-enable if DB connectivity tracing is required
#COPY dmllib.trace.php ./lib/dmllib.php

# Copy the custom GOVUK theme
COPY install-theme install-theme
RUN chmod +x install-theme \
    && ./install-theme --copy-only

# Mandatory tools for installation + remote SSH
RUN apt-get update && apt-get install -y --no-install-recommends \
    postgresql-client \
    openssh-server \
    && rm -rf /var/lib/apt/lists/*
    
# Helpful tools for remote debug via SSH
RUN apt-get update && apt-get install -y --no-install-recommends \
    iputils-ping \
    dnsutils \
    net-tools \
    traceroute \
    curl \
    wget \
    && rm -rf /var/lib/apt/lists/*

# Custom installation entry point - will run the entire install process
COPY entry-point-install.sh /app/entry-point.sh
RUN chmod +x /app/entry-point.sh
    
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
    
USER www-data

# For Apache: Update DocumentRoot to point to /var/www/html/public
# Add / update DirectoryIndex just need to server a default html doc for the install app serviec
COPY index.html /var/www/html/index.html
ENV APACHE_CONF="/etc/apache2/sites-available/000-default.conf"
RUN if grep -q "^DocumentRoot " "$APACHE_CONF"; then \
        sed -i 's|^DocumentRoot .*|DocumentRoot /var/www/html/public|' "$APACHE_CONF"; \
    else \
        echo "DocumentRoot /var/www/html/public" >> "$APACHE_CONF"; \
    fi && \
    if grep -q "^DirectoryIndex " "$APACHE_CONF"; then \
        sed -i 's|^DirectoryIndex .*|DirectoryIndex index.html|' "$APACHE_CONF"; \
    else \
        echo "DirectoryIndex index.html" >> "$APACHE_CONF"; \
    fi

USER root
    