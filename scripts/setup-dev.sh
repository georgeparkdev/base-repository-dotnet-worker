#!/usr/bin/env bash
set -euo pipefail

# setup-dev.sh
# Interactive helper to prepare the project for local development.
# - Optionally installs BMAD via `npx bmad-method@alpha install`
# - Optionally builds docker images from `docker-compose.yml`

# Ensure we are run from the repository root for consistency with other scripts
ROOT_FILE="DotnetWorker.slnx"
ROOT_DIR="$(pwd)"

if [ ! -f "$ROOT_FILE" ]; then
  echo "Error: Please run this script from repository root (missing $ROOT_FILE)."
  exit 1
fi

yes_no_prompt() {
  local prompt="$1"
  local default="${2:-N}"
  while true; do
    read -r -p "$prompt [y/N]: " ans || true
    ans="${ans:-$default}"
    case "$ans" in
      [Yy]* ) return 0 ;;
      [Nn]* ) return 1 ;;
      * ) echo "Please answer y or n." ;;
    esac
  done
}

echo "Development setup helper"

if yes_no_prompt "Prepare project for BMAD (run 'npx bmad-method@alpha install')?"; then
  if command -v npx >/dev/null 2>&1; then
    echo "Running: npx bmad-method@alpha install"
    (cd "$ROOT_DIR" && npx bmad-method@alpha install)
  else
    echo "'npx' not found. Please install Node.js (which includes npx) and run: npx bmad-method@alpha install" >&2
  fi
else
  echo "Skipping BMAD install."
fi

if yes_no_prompt "Build docker images from docker-compose.yml?"; then
  if command -v docker >/dev/null 2>&1; then
    # Prefer new `docker compose` if available
    if docker compose version >/dev/null 2>&1; then
      echo "Building docker images with 'docker compose build'..."
      (cd "$ROOT_DIR" && docker compose -f docker-compose.yml build)
    elif command -v docker-compose >/dev/null 2>&1; then
      echo "Building docker images with 'docker-compose build'..."
      (cd "$ROOT_DIR" && docker-compose -f docker-compose.yml build)
    else
      echo "Docker is installed but neither 'docker compose' nor 'docker-compose' appears to be available." >&2
    fi
  else
    echo "'docker' not found. Please install Docker and ensure it's running." >&2
  fi
else
  echo "Skipping docker image build."
fi

echo "Done."
