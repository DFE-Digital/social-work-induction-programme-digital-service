#!/bin/sh
# stop on error (-e) and on unset var expansion (-u)
set -eu
# enable pipefail *only* if the shell supports it
( set -o pipefail 2>/dev/null ) && set -o pipefail

# Start ssh for kudu / tunnelling support
/usr/sbin/sshd

# Exec the main process passed to us (whatever is specified in CMD)
exec "$@"
