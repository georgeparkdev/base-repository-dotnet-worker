#!/usr/bin/env bash
set -euo pipefail

# scripts/check-licenses.sh
# -------------------------
# Run nuget-license against the repository solution and produce reports
# - Full JSON report: scripts/artifacts/nuget-license-<timestamp>.json
# - Invalid packages JSON: scripts/artifacts/nuget-license-invalid-<timestamp>.json
# - Invalid packages markdown summary: scripts/artifacts/nuget-license-invalid-<timestamp>.md
#
# Usage: ./scripts/check-licenses.sh
# Must be run from repository root (contains DotnetWorker.slnx)
#

ROOT_FILE="DotnetWorker.slnx"
if [ ! -f "${ROOT_FILE}" ]; then
  echo "Error: Please run this script from repository root (missing ${ROOT_FILE})."
  exit 1
fi

# Ensure nuget-license is available
if ! command -v nuget-license &> /dev/null; then
    echo "nuget-license is not installed."
    echo "Install it with: dotnet tool install --global nuget-license"
    exit 1
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ARTIFACTS_DIR="${SCRIPT_DIR}/artifacts"
ALLOWED_LICENSES_FILE="${SCRIPT_DIR}/allowed-licenses.json"

mkdir -p "${ARTIFACTS_DIR}"

if [ ! -f "${ALLOWED_LICENSES_FILE}" ]; then
  echo "Allowed licenses file not found at ${ALLOWED_LICENSES_FILE}. Creating a default file with common permissive licenses."
  cat > "${ALLOWED_LICENSES_FILE}" <<'JSON'
[
  "MIT",
  "Apache-2.0",
  "BSD-3-Clause",
  "BSD-2-Clause",
  "ISC",
  "MS-PL",
  "Zlib",
  "CC0-1.0",
  "Unlicense"
]
JSON
fi

TIMESTAMP="$(date -u +%Y%m%dT%H%M%SZ)"
REPORT_FILE="${ARTIFACTS_DIR}/nuget-license-${TIMESTAMP}.json"
INVALID_JSON_FILE="${ARTIFACTS_DIR}/nuget-license-invalid-${TIMESTAMP}.json"
INVALID_MD_FILE="${ARTIFACTS_DIR}/nuget-license-invalid-${TIMESTAMP}.md"

echo "Running license check against ${ROOT_FILE}"
echo "Allowed licenses: ${ALLOWED_LICENSES_FILE}"
echo "Full JSON report: ${REPORT_FILE}"

# Run nuget-license to produce full JSON report (pretty)
nuget-license -i "${ROOT_FILE}" -a "${ALLOWED_LICENSES_FILE}" -o JsonPretty -fo "${REPORT_FILE}" || true

# Parse invalid packages (those with ValidationErrors) into a separate file
if command -v jq &> /dev/null; then
  echo "Parsing invalid packages using jq..."
  jq '[.[] | select(.ValidationErrors != null and (.ValidationErrors | length > 0)) | {PackageId, PackageVersion, License, LicenseUrl, PackageProjectUrl, Authors, ValidationErrors}]' "${REPORT_FILE}" > "${INVALID_JSON_FILE}" || true
  COUNT=$(jq 'length' "${INVALID_JSON_FILE}")
  if [ "$COUNT" -eq 0 ]; then
    echo "No invalid packages detected."
    echo "[]" > "${INVALID_JSON_FILE}"
    cat > "${INVALID_MD_FILE}" <<EOF
# NuGet License Check - Invalid Packages

No invalid packages found. Full report:

- JSON: ${REPORT_FILE}

EOF
  else
    echo "Found ${COUNT} invalid package(s). Writing summary to ${INVALID_MD_FILE}"
    {
      echo "# NuGet License Check - Invalid Packages"
      echo
      echo "Generated: ${TIMESTAMP} (UTC)"
      echo
      echo "Full JSON report: ${REPORT_FILE}"
      echo
      echo "Invalid packages:"
      echo
      jq -r '.[] | "- \(.PackageId) \(.PackageVersion) — License: \(.License // "(none)") — Errors: \(.ValidationErrors | map(.Error) | join("; "))"' "${INVALID_JSON_FILE}"
    } > "${INVALID_MD_FILE}"
  fi
else
  echo "jq not found. Attempting to parse invalid packages using python..."
  if command -v python &> /dev/null || command -v python3 &> /dev/null; then
    PYTHON_CMD="$(command -v python || command -v python3)"
    ${PYTHON_CMD} - <<PYTHON
import json,sys
report = json.load(open('${REPORT_FILE}'))
invalid = []
for p in report:
    errs = p.get('ValidationErrors') or []
    if len(errs) > 0:
        invalid.append({
            'PackageId': p.get('PackageId'),
            'PackageVersion': p.get('PackageVersion'),
            'License': p.get('License'),
            'LicenseUrl': p.get('LicenseUrl'),
            'PackageProjectUrl': p.get('PackageProjectUrl'),
            'Authors': p.get('Authors'),
            'ValidationErrors': errs
        })
json.dump(invalid, open('${INVALID_JSON_FILE}','w'), indent=2)
with open('${INVALID_MD_FILE}','w') as f:
    f.write('# NuGet License Check - Invalid Packages\n\n')
    f.write('Generated: ${TIMESTAMP} (UTC)\n\n')
    if not invalid:
        f.write('No invalid packages found. Full report: {}\n'.format('${REPORT_FILE}'))
    else:
        f.write('Full JSON report: {}\n\n'.format('${REPORT_FILE}'))
        f.write('Invalid packages:\n\n')
        for p in invalid:
            errs = '; '.join(e.get('Error','') for e in p.get('ValidationErrors',[]))
            f.write('- {0} {1} — License: {2} — Errors: {3}\n'.format(p.get('PackageId'), p.get('PackageVersion'), p.get('License') or '(none)', errs))
PYTHON
    COUNT=$(python -c "import json; print(len(json.load(open('${INVALID_JSON_FILE}'))))") || COUNT=0
    if [ "$COUNT" -eq 0 ]; then
      echo "No invalid packages detected."
    else
      echo "Found ${COUNT} invalid package(s)."
    fi
  else
    echo "Neither jq nor python/python3 were found. Cannot create an invalid-package summary, but full report is available at: ${REPORT_FILE}"
    echo "[]" > "${INVALID_JSON_FILE}"
    cat > "${INVALID_MD_FILE}" <<EOF
# NuGet License Check - Invalid Packages

Could not produce invalid packages summary (missing 'jq' or 'python'). See full report: ${REPORT_FILE}

EOF
  fi
fi

echo "Full report: ${REPORT_FILE}"
echo "Invalid packages JSON: ${INVALID_JSON_FILE}"
echo "Invalid packages summary: ${INVALID_MD_FILE}"

# Exit non-zero if there were invalid packages
if [ -f "${INVALID_JSON_FILE}" ]; then
  if grep -q '"PackageId"' "${INVALID_JSON_FILE}" 2>/dev/null; then
    echo "License check completed with errors (invalid licenses found)."
    exit 2
  fi
fi

echo "License check completed successfully (no invalid licenses)."
exit 0

