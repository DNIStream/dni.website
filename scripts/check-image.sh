#!/usr/bin/env bash
# Fails when the image or the running container exceeds its budget: image under 30 MB, memory in
# use under 48 MB, and never running as root. Usage: check-image.sh <image> <container>
set -euo pipefail
export MSYS_NO_PATHCONV=1

IMAGE=$1
CONTAINER=$2
MAX_IMAGE_BYTES=30000000
MAX_MEMORY_BYTES=$((48 * 1024 * 1024))
failed=0

image_bytes=$(docker image inspect --format '{{.Size}}' "$IMAGE")
# docker stats reports memory as e.g. "3.4MiB / 48MiB"; convert the first figure to bytes.
memory=$(docker stats --no-stream --format '{{.MemUsage}}' "$CONTAINER" | awk '{ print $1 }')
memory_bytes=$(awk -v m="$memory" 'BEGIN {
  n = m + 0; unit = m; sub(/^[0-9.]+/, "", unit)
  f = (unit == "KiB" ? 1024 : unit == "MiB" ? 1048576 : unit == "GiB" ? 1073741824 : unit == "kB" ? 1000 : unit == "MB" ? 1000000 : 1)
  printf "%d", n * f }')
user=$(docker exec "$CONTAINER" id -u)

printf 'Image size: %.1f MB (limit 30 MB)\n' "$(awk -v b="$image_bytes" 'BEGIN { print b / 1000000 }')"
printf 'Memory:     %s (limit 48 MiB)\n' "$memory"
printf 'User id:    %s\n' "$user"

if [ "$image_bytes" -ge "$MAX_IMAGE_BYTES" ]; then echo "FAIL: the image is 30 MB or more" >&2; failed=1; fi
if [ "$memory_bytes" -ge "$MAX_MEMORY_BYTES" ]; then echo "FAIL: memory in use is 48 MiB or more" >&2; failed=1; fi
if [ "$user" = "0" ]; then echo "FAIL: the server runs as root" >&2; failed=1; fi
exit "$failed"
