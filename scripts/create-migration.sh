#!/usr/bin/env bash
set -euo pipefail

# Create a new EF Core migration in src/DotnetWorker.Infrastructure/Data/Migrations
# Usage: ./scripts/create-migration.sh <MigrationName>

ROOT_FILE="DotnetWorker.slnx"
INFRA_PROJECT="src/DotnetWorker.Infrastructure"
STARTUP_PROJECT="src/DotnetWorker.WorkerService"
OUTPUT_DIR="Data/Migrations"

if [ ! -f "$ROOT_FILE" ]; then
  echo "Error: Please run this script from repository root (missing $ROOT_FILE)."
  exit 1
fi

if [ "$#" -lt 1 ]; then
  echo "Usage: $0 <MigrationName>"
  exit 1
fi

MIGRATION_NAME="$1"
shift

# Ensure migrations directory exists
mkdir -p "$INFRA_PROJECT/$OUTPUT_DIR"

# Validate required tools
if ! command -v dotnet >/dev/null 2>&1; then
  echo "Error: 'dotnet' is not installed or not on PATH. Please install the .NET SDK: https://dotnet.microsoft.com/download"
  exit 1
fi

if ! dotnet ef --version >/dev/null 2>&1; then
  echo "Error: 'dotnet ef' CLI is not available. Install with: dotnet tool install --global dotnet-ef (or add as a local tool)"
  exit 1
fi

echo "Creating migration '$MIGRATION_NAME' in $INFRA_PROJECT (output: $OUTPUT_DIR) using startup project $STARTUP_PROJECT..."

dotnet ef migrations add "$MIGRATION_NAME" \
  --project "$INFRA_PROJECT" \
  --startup-project "$STARTUP_PROJECT" \
  --output-dir "$OUTPUT_DIR" "$@"

echo "Migration '$MIGRATION_NAME' created."
