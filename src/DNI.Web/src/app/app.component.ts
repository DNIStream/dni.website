import { Component, Inject, APP_ID, PLATFORM_ID, OnInit } from '@angular/core';
import { environment } from 'environments/environment.prod';
import { Router } from '@angular/router';

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
    private appId: string,
    private router: Router
  ) {
    console.log(platformId);
  }

  ngOnInit(): void {
    this.version = environment.version;
    this.versionText = environment.versionText;

    this.registerRouteScroll();
  }

  private registerRouteScroll(): void {
    this.router.events
      .subscribe((evt) => {
        // Scroll to the top of the page
        window.scrollTo(0, 0);
      });
  }
}
