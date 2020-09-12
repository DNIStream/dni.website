import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { environment } from 'environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PlatformService {

  public get isBrowser() {
    return isPlatformBrowser(this.platformId);
  }

  public get isServer() {
    return !isPlatformBrowser(this.platformId);
  }

  public get isDev() {
    return !environment.production;
  }

  public get isProd() {
    return environment.production;
  }

  constructor(
    @Inject(PLATFORM_ID) protected platformId: object
  ) { }
}
