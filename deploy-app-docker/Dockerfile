FROM alpine:3.21.3

RUN apk update && apk add \
  iputils \
  bind-tools \
  net-tools \
  traceroute \
  curl \
  wget \
  busybox-extras \
  postgresql-client \
  openssh \
  coreutils

RUN mkdir -p /var/run/sshd /app
COPY entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh
# Modify to run SSH on port 80 for web app service compatibility
RUN sed -i 's/^#\?Port.*22/Port 80/' /etc/ssh/sshd_config

EXPOSE 80

CMD ["/app/entrypoint.sh"]
