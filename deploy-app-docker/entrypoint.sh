if [ -n "$SSH_CERT_BASE64" ]; then
  echo "$SSH_CERT_BASE64" | base64 -d > /etc/ssh/ssh_host_rsa_key
  chmod 600 /etc/ssh/ssh_host_rsa_key
  ssh-keygen -y -f /etc/ssh/ssh_host_rsa_key > /etc/ssh/ssh_host_rsa_key.pub
else
  ssh-keygen -A
fi
exec /usr/sbin/sshd -D
