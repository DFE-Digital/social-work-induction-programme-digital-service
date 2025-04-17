FROM tsl0922/ttyd:latest

# Set non-interactive mode for apt-get
ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get install -y --no-install-recommends \
  iputils-ping \
  dnsutils \
  telnet \
  net-tools \
  traceroute \
  curl \
  wget \
  nmap \
  iperf \
  tcpdump \
  netcat \
#  mysql-client \
  mariadb-client \
  postgresql-client \
  mtr-tiny && rm -rf /var/lib/apt/lists/*

  CMD ["/bin/sh", "-c", "exec ttyd -W -c \"${TRACE_WEBAPP_ADMIN_USER}:${TRACE_WEBAPP_ADMIN_PASSWORD}\" bash"]
  