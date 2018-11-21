import { Component, Inject, APP_ID, PLATFORM_ID, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';

import { environment } from 'environments/environment';

@Component({
  selector: 'dni-root',
  templateUrl: './app.component.html',
  styleUrls: [
    './app.component.scss'
  ]
})
export class AppComponent implements OnInit {
  public version: string;
  public versionText: string;

  constructor(
    @Inject(PLATFORM_ID) protected platformId: Object,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.version = environment.version;
    this.versionText = environment.versionText;
  }
}
