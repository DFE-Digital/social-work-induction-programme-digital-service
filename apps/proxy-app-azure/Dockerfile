FROM alpine:3.21.3 AS proxy-service

ARG AZURE_KUDU_SSH_USER
ARG FULL_TAG

# Install lighttpd - lightweight / fast web server
RUN apk add --no-cache lighttpd

# Copy hardened lighttpd config
COPY lighttpd.conf /etc/lighttpd/lighttpd.conf
RUN mkdir -p /apps
COPY entry-point.sh /apps/entry-point.sh
RUN chmod +x /apps/entry-point.sh

# Create the default webroot (it already exists, but ensure permissions)
RUN mkdir -p /var/www/localhost/htdocs \
    && chown -R lighttpd:lighttpd /var/www/localhost/htdocs

COPY index.html /var/www/localhost/htdocs
RUN echo $FULL_TAG > /var/www/localhost/htdocs/version.txt \
    && chown -R lighttpd:lighttpd /var/www/localhost/htdocs

# Mandatory tools for installation + remote SSH
RUN apk add --no-cache \
    openssh \
    shadow

USER ${AZURE_KUDU_SSH_USER}

# Support kudu tools SSH for remote debug
COPY sshd_config /etc/ssh/sshd_config
RUN --mount=type=secret,id=AZURE_KUDU_SSH_PASSWORD \
    export AZURE_KUDU_SSH_PASSWORD="$(cat /run/secrets/AZURE_KUDU_SSH_PASSWORD)" && \
    echo "${AZURE_KUDU_SSH_USER}:${AZURE_KUDU_SSH_PASSWORD}" | chpasswd && \
    : > /etc/motd && \
    mkdir -p /var/run/sshd && chmod 0755 /var/run/sshd && \
    ssh-keygen -A

# Helpful tools for remote debug via SSH
RUN apk add --no-cache \
    iputils \
    bind-tools \
    net-tools \
    traceroute \
    postgresql-client \
    azure-cli

# Copy backup and restore scripts into the container
COPY run-backup.sh /usr/local/bin/run-backup.sh
COPY run-restore.sh /usr/local/bin/run-restore.sh

RUN chmod +x /usr/local/bin/run-backup.sh
RUN chmod +x /usr/local/bin/run-restore.sh

ENTRYPOINT ["/apps/entry-point.sh"]

CMD ["lighttpd", "-D", "-f", "/etc/lighttpd/lighttpd.conf"]

EXPOSE 80
