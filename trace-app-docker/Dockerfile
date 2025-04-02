FROM tsl0922/ttyd:latest

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
  mysql-client \
  mariadb-client \
  postgresql-client \
  mtr-tiny && rm -rf /var/lib/apt/lists/*

  CMD ["/bin/sh", "-c", "exec ttyd -W -c \"${TTYD_USER}:${TTYD_PASSWORD}\" bash"]
  