#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

if [[ -z "${COATIFY_TEST_ADMIN_CONNECTION:-}" ]]; then
    docker compose --project-directory "$repo_root" up -d --wait postgres
fi

dotnet test "$repo_root/Coatify.Tests/Coatify.Tests.csproj" "$@"
