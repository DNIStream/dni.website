#!/usr/bin/env bash
# Builds the production image, runs it with production limits, and runs the E2E suite against it.
# Extra arguments go to Playwright, e.g. `npm run test:e2e -- tests/e2e/archive.spec.js`.
set -euo pipefail
# Git Bash would otherwise rewrite container paths such as /tmp into Windows paths.
export MSYS_NO_PATHCONV=1

IMAGE=dni-website:e2e
CONTAINER=dni-website-e2e
PORT=${E2E_PORT:-8089}

cleanup() { docker rm -f "$CONTAINER" >/dev/null 2>&1 || true; }
trap cleanup EXIT

# The same offline build as the image's: tests/e2e/every-page.spec.js lists its pages from dist/.
FEED_OFFLINE=1 npm run -s build >/dev/null
docker build --target site --build-arg FEED_OFFLINE=1 -t "$IMAGE" .
cleanup
docker run -d --name "$CONTAINER" \
  --read-only --tmpfs /tmp \
  --memory 48m --memory-swap 48m --cpus 0.25 \
  --cap-drop ALL --security-opt no-new-privileges:true \
  -p "127.0.0.1:${PORT}:8080" "$IMAGE" >/dev/null

for _ in $(seq 1 50); do
  if curl -fsS "http://127.0.0.1:${PORT}/healthz" >/dev/null 2>&1; then break; fi
  sleep 0.2
done
curl -fsS "http://127.0.0.1:${PORT}/healthz" >/dev/null

BASE_URL="http://localhost:${PORT}" npx playwright test "$@"

if [ -f scripts/check-image.sh ]; then
  bash scripts/check-image.sh "$IMAGE" "$CONTAINER"
fi
