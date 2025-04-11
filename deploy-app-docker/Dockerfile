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

EXPOSE 22

CMD ["/app/entrypoint.sh"]
