FROM mcr.microsoft.com/azure-cli

USER root

# Install postgresql-client
RUN apt-get update && \
    apt-get install -y postgresql-client && \
    rm -rf /var/lib/apt/lists/*

COPY run_backup.sh /usr/local/bin/run_backup.sh
COPY run_restore.sh /usr/local/bin/run_restore.sh

RUN chmod +x /usr/local/bin/run_backup.sh /usr/local/bin/run_restore.sh

# The container will idle when it starts. The command to run
# will be provided by the GitHub Action that triggers the job.
ENTRYPOINT [ "tail", "-f", "/dev/null" ]