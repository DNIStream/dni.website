# syntax=docker/dockerfile:1

FROM node:24-alpine AS build
WORKDIR /app
ENV PLAYWRIGHT_SKIP_BROWSER_DOWNLOAD=1
COPY package.json package-lock.json ./
RUN npm ci --no-audit --no-fund
COPY eleventy.config.js ./
COPY lib lib
COPY scripts/optimize-assets.mjs scripts/optimize-assets.mjs
COPY site site
ARG SITE_URL=http://localhost:8080
ARG PLAUSIBLE_DOMAIN=
# 1 builds from the committed feed snapshot only; 0 tries the live feed first.
ARG FEED_OFFLINE=0
RUN SITE_URL="$SITE_URL" PLAUSIBLE_DOMAIN="$PLAUSIBLE_DOMAIN" FEED_OFFLINE="$FEED_OFFLINE" npm run build

FROM alpine:3.22 AS site
RUN apk add --no-cache nginx \
  && rm -rf /etc/nginx/http.d /usr/share/nginx/html /var/lib/nginx/html
COPY docker/nginx.conf /etc/nginx/nginx.conf
COPY docker/security-headers.conf /etc/nginx/security-headers.conf
COPY --from=build /app/dist /usr/share/nginx/html
USER nginx
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget -qO- http://127.0.0.1:8080/healthz >/dev/null || exit 1
CMD ["nginx", "-g", "daemon off;"]
