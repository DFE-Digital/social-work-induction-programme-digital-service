#!/bin/bash
set -euo pipefail

# The Moodle image build process requires these additional files to be sourced from the repo
PHP_CONFIG_FILE="apps/moodle-docker/config.php"
MOODLE_SCRIPTS_DIR="apps/moodle-ddev/.ddev/commands/web"
DESTINATION_DIR="apps/moodle-apps-azure/"

errors=0

if [ ! -f "$PHP_CONFIG_FILE" ]; then
  echo "Error: required file '$PHP_CONFIG_FILE' not found." >&2
  errors=1
fi

if [ ! -d "$MOODLE_SCRIPTS_DIR" ]; then
  echo "Error: directory '$MOODLE_SCRIPTS_DIR' not found." >&2
  errors=1
elif [ -z "$(ls -A "$MOODLE_SCRIPTS_DIR")" ]; then
    echo "Error: no files to copy in '$MOODLE_SCRIPTS_DIR'." >&2
    errors=1
fi

# If any check failed, abort now
if [ "$errors" -ne 0 ]; then
  echo "One or more required items are missing. Aborting..." >&2
  exit 1
fi

echo "Copying additional files for Docker Mooodle image build..."
cp "$PHP_CONFIG_FILE" "$DESTINATION_DIR"
cp "$MOODLE_SCRIPTS_DIR"/* "$DESTINATION_DIR"

# Run temporary postgres DB in the background for the build process
# Moodle requires this for upgrading, so we provide it for the install as well
docker run --rm -d \
    --name temp-postgres \
    -e POSTGRES_USER=postgres \
    -e POSTGRES_PASSWORD=postgres \
    -e POSTGRES_DB=moodle \
    -p 5432:5432 \
    postgres:17.4

