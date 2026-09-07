#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"
INSTALL_DIR="${1:-${REPO_ROOT}/dist/Peach}"

command -v dotnet >/dev/null 2>&1 || {
    echo "dotnet is required but was not found in PATH." >&2
    exit 1
}

mkdir -p "${INSTALL_DIR}"

echo "Publishing Peach to ${INSTALL_DIR}"
dotnet restore "${REPO_ROOT}/Peach.Build.csproj"
dotnet publish "${REPO_ROOT}/Peach.Build.csproj" \
    --configuration Release \
    --no-restore \
    --output "${INSTALL_DIR}"

# These assemblies are loaded as Peach extensions by the legacy installation
# layout. They must remain under Plugins rather than beside Peach itself.
PLUGIN_DIR="${INSTALL_DIR}/Plugins"
mkdir -p "${PLUGIN_DIR}"
cp -R "${REPO_ROOT}/pro/Peach/Plugins/." "${PLUGIN_DIR}/"
cp "${REPO_ROOT}/llm/Core/bin/Release/net8.0/Peach.LLM.dll" "${PLUGIN_DIR}/"
cp "${REPO_ROOT}/llm/Validations/Common/bin/Release/net8.0/Peach.LLM.Validations.Common.dll" "${PLUGIN_DIR}/"

echo
echo "Peach has been installed to: ${INSTALL_DIR}"
echo "Plugins have been installed to: ${PLUGIN_DIR}"
echo "Run: ${INSTALL_DIR}/Peach --help"
