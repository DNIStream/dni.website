# Contract: Production composition

Built **after** the user's local container test (research R15).

`compose.production.yaml` at the repository root, with no comments about hosting:

```yaml
x-webapp:
  app: dnistream
  images:
    web: { build: { context: ., dockerfile: Dockerfile, target: site, args: [SITE_URL, PLAUSIBLE_DOMAIN] } }
  publish: [web]
  services:
    web:
      mem_mb: 48
      read_only: true
      tmpfs: [/tmp]
      probe: { http: /healthz, expect: ok, timeout: 60 }
      backup: { driver: none }
  analytics: true
  env:
    required: [SITE_URL, PLAUSIBLE_DOMAIN]
services:
  web:
    image: ${WEBAPP_IMAGE_WEB}
    ports: ["127.0.0.1:${WEBAPP_PORT_WEB}:8080"]
```

- No `env_file`: the running container needs no settings; the canonical origin and the analytics
  domain are build arguments.
- `.env.example` is the only committed environment file and holds exactly
  `SITE_URL=https://example.com` and `PLAUSIBLE_DOMAIN=example.com`. `.gitignore` ignores `.env.*`
  except `.env.example`.
- Any change to this file bumps the site's version (`VERSION`) and is released as a `vX.Y.Z` tag.
