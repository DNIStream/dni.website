import { Component, Inject, APP_ID, PLATFORM_ID, OnInit } from '@angular/core';
import { environment } from 'environments/environment.prod';

@Component({
  selector: 'dni-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  public version: string;
  public versionText: string;

  constructor(
    @Inject(PLATFORM_ID)
    protected platformId: Object,
    @Inject(APP_ID)
    private appId: string
  ) {
    console.log(platformId);
  }

  ngOnInit(): void {
    this.version = environment.version;
    this.versionText = environment.versionText;
  }
}
