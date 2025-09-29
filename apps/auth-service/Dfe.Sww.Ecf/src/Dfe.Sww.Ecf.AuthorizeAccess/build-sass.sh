#!/bin/bash

# Try to find sass in common version manager locations
find_sass() {
  # First try: use command -v to find sass in current PATH
  if command -v sass >/dev/null 2>&1; then
    echo "✅ sass found in PATH"
    return 0
  fi

  # Second try: check common version manager paths
  local common_paths=(
    "$HOME/.local/share/mise/shims"
    "$HOME/.asdf/shims"
    "/opt/homebrew/bin"
    "/home/linuxbrew/.linuxbrew/bin"
    "/usr/local/bin"
    "$HOME/.local/bin"
    "$HOME/.npm-global/bin"
  )

  for path_dir in "${common_paths[@]}"; do
    if [ -x "$path_dir/sass" ]; then
      echo "✅ sass found in ${path_dir}"
      export PATH="$path_dir:$PATH"
      return 0
    fi
  done

  # Third try: check if sass is available via npm/npx
  if command -v npx >/dev/null 2>&1; then
    echo "✅ using npx for sass"
    export SASS_CMD="npx sass"
    return 0
  fi

  return 1
}

if ! find_sass; then
  echo "❌ ERROR: Sass not found. Install via: npm install -g sass"
  exit 1
fi

# Use the found sass command (either direct sass or npx sass)
SASS_CMD="${SASS_CMD:-sass}"
$SASS_CMD -q wwwroot/Styles/site.scss wwwroot/Styles/site.css
