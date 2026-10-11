#!/usr/bin/env bash
# Reports the size and resource use of the local container started by `npm run container:up`.
set -euo pipefail
export MSYS_NO_PATHCONV=1

IMAGE=dni-website:local
CONTAINER=$(docker compose ps -q web 2>/dev/null)
if [ -z "$CONTAINER" ]; then
  echo "The container is not running: start it with npm run container:up" >&2
  exit 1
fi
PORT=$(docker compose port web 8080 2>/dev/null | sed 's/.*://')

bytes=$(docker image inspect --format '{{.Size}}' "$IMAGE")
echo "Image:   $IMAGE, $(awk -v b="$bytes" 'BEGIN { printf "%.1f MB", b / 1000000 }')"
docker stats --no-stream --format 'Memory:  {{.MemUsage}} ({{.MemPerc}})
CPU:     {{.CPUPerc}}' "$CONTAINER"
echo "User:    $(docker exec "$CONTAINER" id -un)"
echo "Port:    http://localhost:${PORT}"
echo "Health:  $(curl -fsS "http://127.0.0.1:${PORT}/healthz" || echo "no answer")"
