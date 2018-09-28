import { Component, Inject, APP_ID, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { ContactService } from 'app/services/contact.service';

@Component({
  selector: 'dni-root',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  constructor(
    @Inject(PLATFORM_ID)
    private platformId: Object,
    @Inject(APP_ID)
    private appId: string,
    private contactService: ContactService
  ) {
    console.log(this.appId + ' ' + this.platformId);
  }

  public get title() {
    return '\'' + this.appId + '\' running in \'' + (isPlatformBrowser(this.platformId) ? 'browser' : 'server') + '\' mode';
  }
}
