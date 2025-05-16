#!/bin/sh
# stop on error (-e) and on unset var expansion (-u)
set -eu
# enable pipefail *only* if the shell supports it
( set -o pipefail 2>/dev/null ) && set -o pipefail

# Support SSH for troubleshooting
echo "Starting SSH..."
/usr/sbin/sshd

# Exec the main process passed to us (whatever is specified in CMD)
exec "$@"
