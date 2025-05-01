#!/usr/bin/env bash
set -euo pipefail

usage() {
  echo "Usage: $0 INPUT_FILE"
  exit 1
}

if (( $# != 1 )); then
  usage
fi

infile="$1"

if [[ ! -f "$infile" ]]; then
  echo "ERROR: File not found: $infile" >&2
  exit 2
fi

while IFS= read -r line; do
  # Skip empty lines or comments
  [[ -z "$line" || "${line:0:1}" == "#" ]] && continue
  # Directly export KEY=VALUE
  export "$line"
done < "$infile"

echo "Imported environment from: $infile"
