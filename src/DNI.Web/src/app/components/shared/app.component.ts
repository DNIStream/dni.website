import { Component, OnInit } from '@angular/core';

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

  constructor() { }

  ngOnInit(): void {
    this.version = environment.version;
    this.versionText = environment.versionText;
  }
}
