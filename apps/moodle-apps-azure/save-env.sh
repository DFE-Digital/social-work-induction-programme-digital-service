#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat <<EOF
Usage: $0 OUTPUT_FILE [EXCLUDE_VAR1 EXCLUDE_VAR2 ...]
Dumps all environment variables to OUTPUT_FILE, skipping any whose
names contain any of the EXCLUDE_VARs.
EOF
  exit 1
}

if (( $# < 1 )); then
  usage
fi

outfile="$1"
shift

# Build a regex alternation from the exclude list, if any
if (( $# > 0 )); then
  esc=()
  for v in "$@"; do
    # escape any regex metachars
    esc+=( "$(printf '%s' "$v" | sed 's/[][\.*^$(){}?+|/]/\\&/g')" )
  done
  # pattern to match any key that contains one of the exclude names
  pattern_list="$(IFS='|'; echo "${esc[*]}")"
  pattern="^([^=]*(${pattern_list})[^=]*)="
else
  # match nothing
  pattern='^$'
fi

# Dump, filtering out any key that contains one of the excludes
printenv | grep -v -E "$pattern" > "$outfile"

echo "Wrote environment to $outfile (excluded patterns: ${*:-none})"
