
ARG FULL_MOODLE_IMAGE_VERSION

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
    openssh \
    && rm -rf /var/lib/apt/lists/*
  
COPY entry-point-azure-install.sh /app/entry-point.sh
RUN chmod +x /app/entry-point.sh

# Support kudu tools SSH for remote debug
COPY sshd_config /etc/ssh/sshd_config
RUN echo "root:Docker!" | chpasswd \
  && > /etc/motd \
  && chmod +x /app/entrypoint.sh \
  && cd /etc/ssh/ \
  && ssh-keygen -A
