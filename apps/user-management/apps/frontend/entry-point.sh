#!/bin/sh
# stop on error (-e) and on unset var expansion (-u)
set -eu
# enable pipefail *only* if the shell supports it
( set -o pipefail 2>/dev/null ) && set -o pipefail

# Support SSH for troubleshooting
echo "Starting SSH..."
/usr/sbin/sshd

if [ "$BASIC_AUTH_ENABLED" = 'true' ]; then
    # Configure basic auth to restrict access / prevent user management from being indexed
    echo "Configuring basic auth..."
    htpasswd -b -c /etc/nginx/.htpasswd "$BASIC_AUTH_USER" "$BASIC_AUTH_PASSWORD" > /dev/null 2>&1
    cp /App/nginx-basic-auth.conf /etc/nginx/http.d/default.conf
fi

# Launch user management
dotnet Dfe.Sww.Ecf.Frontend.dll &

echo "Waiting for user management to be ready..."
ENDPOINT=http://localhost:5000/version.txt
EXPECTED_RESPONSE=$(cat /App/wwwroot/version.txt)
TOTAL_TIMEOUT=10
TIMEOUT_PER_REQUEST=2 # seconds
SLEEP_INTERVAL=1      # seconds between retries

start_time=$(date +%s)
RESPONSE=""

while true; do
    NOW=$(date +%s)
    ELAPSED=$(( NOW - start_time ))

    if [ "$ELAPSED" -ge "$TOTAL_TIMEOUT" ]; then
        echo "❌ Timed out after ${TOTAL_TIMEOUT}s waiting for $ENDPOINT" >&2
        exit 1
    fi

    # Run the request, but don’t abort the script on timeout/failure
    if BODY=$(curl --silent --show-error --max-time $TIMEOUT_PER_REQUEST "$ENDPOINT"); then
        if [ -n "$BODY" ]; then
            if [ "$BODY" == "$EXPECTED_RESPONSE" ]; then
                RESPONSE="$BODY"
                echo "✅ Got correct endpoint response '$EXPECTED_RESPONSE' after ${ELAPSED}s:"
                echo "$RESPONSE"
                break
            else
                echo "⏳ Got incorrect endpoint response '$BODY', expected '$EXPECTED_RESPONSE' after ${ELAPSED}s:"
            fi
        fi
    else
        echo "⏳ Attempt at ${ELAPSED}s failed or timed out (allowed ${TIMEOUT_PER_REQUEST}s)."
    fi

    sleep $SLEEP_INTERVAL
done

# Exec the main process passed to us (whatever is specified in CMD - usually nginx)
exec "$@"
