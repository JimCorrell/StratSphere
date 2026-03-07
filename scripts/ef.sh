#!/usr/bin/env bash
set -euo pipefail

# Use the locally installed .NET SDK/runtime so EF tooling resolves correctly.
export PATH="$HOME/.dotnet:$HOME/.dotnet/tools:$PATH"

if [[ $# -eq 0 ]]; then
  echo "Usage: scripts/ef.sh <ef arguments>"
  echo "Example: scripts/ef.sh migrations add InitialCreate --project ../StratSphere.Data"
  exit 1
fi

dotnet ef "$@"
