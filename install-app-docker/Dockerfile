
ARG FULL_MOODLE_IMAGE_VERSION
ARG AZURE_KUDU_SSH_USER
ARG AZURE_KUDU_SSH_PASSWORD

# The install image is based off exactly the same image as the Moodle image
FROM ${FULL_MOODLE_IMAGE_VERSION}

RUN apt-get update && apt-get install -y --no-install-recommends \
    iputils-ping \
    dnsutils \
    net-tools \
    traceroute \
    curl \
    wget \
    postgresql-client \
    openssh-server \
    && rm -rf /var/lib/apt/lists/*
  
COPY entry-point-azure.sh /app/entry-point.sh
RUN chmod +x /app/entry-point.sh

# Support kudu tools SSH for remote debug
COPY sshd_config /etc/ssh/sshd_config
RUN --mount=type=secret,id=AZURE_KUDU_SSH_PASSWORD,env=AZURE_KUDU_SSH_PASSWORD \
  echo "Password: $AZURE_KUDU_SSH_PASSWORD" \
  echo "${AZURE_KUDU_SSH_USER}:$AZURE_KUDU_SSH_PASSWORD" | chpasswd \
  && > /etc/motd \
  && cd /etc/ssh/ \
  && ssh-keygen -A
