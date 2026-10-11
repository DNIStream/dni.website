# Contract: Production image

| Property | Requirement |
|---|---|
| Base | `alpine:3.22` with the Alpine `nginx` package (research R1) |
| Contents | nginx, its config, and the built `dist/`; no Node, no build tooling, no source |
| User | runs as `nginx` (non-root) from start; no added capabilities |
| Port | listens on 8080 only |
| Filesystem | works with a read-only root and `/tmp` as tmpfs; writes nothing elsewhere |
| Health | `GET /healthz` → 200 `ok`, `Cache-Control: no-store` |
| Image size | under 30 MB (SC-006), reported by `docker image inspect --format '{{.Size}}'` |
| Memory | under 48 MB in use while serving (SC-006); runs within a 48 MB limit |
| Workers | one worker process |

## Responses

| Request | Headers |
|---|---|
| Static assets (`css js svg png jpg webp avif woff2 ico webmanifest`) | `Cache-Control: public, max-age=31536000, immutable` for fingerprinted assets; `public, max-age=86400` otherwise |
| HTML | `Cache-Control: no-cache` |
| Every response | `X-Content-Type-Options: nosniff`, `X-Frame-Options: DENY`, `Referrer-Policy: strict-origin-when-cross-origin`, `Permissions-Policy: camera=(), microphone=(), geolocation=()`, `Content-Security-Policy` (below) |
| Text types over 1 KB | gzip-compressed when the client accepts it |

Security headers MUST be present on every location, including those that set their own
`Cache-Control` (they are repeated through an included snippet, not inherited).

## Content-Security-Policy

```
default-src 'self';
script-src 'self' https://web3forms.com https://*.hcaptcha.com https://hcaptcha.com;
frame-src https://*.hcaptcha.com https://hcaptcha.com;
style-src 'self' 'unsafe-inline' https://*.hcaptcha.com https://hcaptcha.com;
img-src 'self' https: data:;
media-src https:;
connect-src 'self' https://api.web3forms.com https://*.hcaptcha.com https://hcaptcha.com;
font-src 'self';
form-action https://api.web3forms.com;
base-uri 'self';
frame-ancestors 'none'
```

`img-src https:` and `media-src https:` allow artwork and audio from the podcast host. The
analytics script and its endpoint are first-party (`'self'`).
