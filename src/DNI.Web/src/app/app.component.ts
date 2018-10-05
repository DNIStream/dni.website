import { Component, Inject, APP_ID, PLATFORM_ID } from '@angular/core';

@Component({
  selector: 'dni-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  constructor(
    @Inject(PLATFORM_ID)
    protected platformId: Object,
    @Inject(APP_ID)
    private appId: string,
  ) {
    console.log(platformId);
  }
}
