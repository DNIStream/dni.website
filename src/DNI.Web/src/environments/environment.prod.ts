const pkgJson = require('../../package.json');
export const environment = {
  production: true,
  webUri: 'http://localhost:4000/',
  apiBaseUri: 'https://api.dnistream.live/',
  recaptchaSiteKey: '6Lc2_XMUAAAAAOd_v5nbtAE_rxgbqspCJJpYQ9zn',
  version: pkgJson.version,
  versionText: 'The MVP'
};
