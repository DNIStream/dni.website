# Contract: Contact form

## Request

`POST https://api.web3forms.com/submit`, `multipart/form-data` (native) or `FormData` via `fetch`
with `Accept: application/json` (enhanced).

| Field | Value | Rule |
|---|---|---|
| `access_key` | `site.web3formsKey` | hidden |
| `subject` | `New DNI Stream contact message` | hidden |
| `from_name` | `DNI Stream Website` | hidden |
| `name` | visitor input | required, max 400 |
| `email` | visitor input | required, `type="email"`, max 400 |
| `message` | visitor input | required, max 5000 |
| `botcheck` | honeypot checkbox | visually hidden, `tabindex="-1"`, `aria-hidden="true"`; must stay unchecked |
| `h-captcha-response` | set by the managed captcha | required before an enhanced submit |

The captcha is `<div class="h-captcha" data-captcha="true"></div>` with
`https://web3forms.com/client/script.js`. A raw hCaptcha site key MUST NOT be used.

## Enhanced behaviour (JavaScript)

| State | Status text (in the `role="status"` `aria-live="polite"` region) | Form |
|---|---|---|
| Captcha not done | `Please complete the "I am human" check before sending.` | not sent; captcha scrolled into view |
| Sending | `Sending…` | Send disabled |
| Success (`ok` and `success: true`) | `Thanks — your message is on its way. We'll be in touch.` | reset; captcha reset |
| Service error | the service's `message`, else `Something went wrong. Please try again.` | values kept; Send re-enabled |
| Network failure | `Network error — please try again in a moment.` | values kept; Send re-enabled |

Native validation runs first; an invalid field is reported by the browser against that field.
A second Send while one is in flight is ignored.

## Without JavaScript

The form posts natively and the visitor sees the form service's own result page.
