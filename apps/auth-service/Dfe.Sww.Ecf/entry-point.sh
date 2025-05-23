#!/bin/sh
# stop on error (-e) and on unset var expansion (-u)
set -eu
# enable pipefail *only* if the shell supports it
( set -o pipefail 2>/dev/null ) && set -o pipefail

###############################################################################
# Guard clauses – fail fast if the env-vars aren’t set                     #
###############################################################################
: "${CONNECTIONSTRINGS__DEFAULTCONNECTION:?CONNECTIONSTRINGS__DEFAULTCONNECTION is not set}"
: "${DB_PASSWORD:?DB_PASSWORD is not set}"

# Construct the full dotnet / db connection string - the password was sourced separately from the key vault
export CONNECTIONSTRINGS__DEFAULTCONNECTION="$(
  printf '%s' "$CONNECTIONSTRINGS__DEFAULTCONNECTION" |
  sed "s|\$\[DB_REPLACE_PASSWORD\]|${DB_PASSWORD}|g"
)"

# Support SSH for troubleshooting
echo "Starting SSH..."
/usr/sbin/sshd

# Exec the main process passed to us (whatever is specified in CMD)
exec su-exec app "$@"
