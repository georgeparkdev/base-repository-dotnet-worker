#!/usr/bin/env bash
set -euo pipefail

# Update the database using EF Core migrations in src/DotnetWorker.Infrastructure
# Usage: ./scripts/update-database.sh [MigrationName]

ROOT_FILE="DotnetWorker.slnx"
INFRA_PROJECT="src/DotnetWorker.Infrastructure"
STARTUP_PROJECT="src/DotnetWorker.WorkerService"

if [ ! -f "$ROOT_FILE" ]; then
  echo "Error: Please run this script from repository root (missing $ROOT_FILE)."
  exit 1
fi

MIGRATION=${1:-}

# Validate required tools
if ! command -v dotnet >/dev/null 2>&1; then
  echo "Error: 'dotnet' is not installed or not on PATH. Please install the .NET SDK: https://dotnet.microsoft.com/download"
  exit 1
fi

if ! dotnet ef --version >/dev/null 2>&1; then
  echo "Error: 'dotnet ef' CLI is not available. Install with: dotnet tool install --global dotnet-ef (or add as a local tool)"
  exit 1
fi

if [ -n "$MIGRATION" ]; then
  echo "Updating database to migration '$MIGRATION'..."
  dotnet ef database update "$MIGRATION" --project "$INFRA_PROJECT" --startup-project "$STARTUP_PROJECT"
else
  echo "Updating database to latest migration..."
  dotnet ef database update --project "$INFRA_PROJECT" --startup-project "$STARTUP_PROJECT"
fi

echo "Database update complete."
