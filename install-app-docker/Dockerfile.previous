FROM alpine:3.21.3

RUN apk update && apk add --no-cache \
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

COPY sshd_config /etc/ssh/sshd_config
COPY entrypoint.sh /app/entrypoint.sh

RUN echo "root:Docker!" | chpasswd \
  && > /etc/motd \
  && chmod +x /app/entrypoint.sh \
  && cd /etc/ssh/ \
  && ssh-keygen -A

RUN mkdir -p /www \
  && echo "Initialised..." > /www/index.html

EXPOSE 80 2222

CMD ["/app/entrypoint.sh"]
