import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

import { Observable } from 'rxjs';

import { TickData } from 'app/services/countdown/tick-data';
import { CountdownService } from 'app/services/countdown/countdown.service';
import { SEOService } from 'app/services/seo/seo.service';

@Component({
  selector: 'dni-root',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  private timer$: Observable<TickData>;

  public tickData: TickData;

  constructor(
    private countdownService: CountdownService,
    @Inject(PLATFORM_ID) protected platformId: Object,
    private seoService: SEOService
  ) { }

  ngOnInit(): void {
    this.seoService.setTitle('Home');

    // Set up the initial timer properties
    this.timer$ = this.countdownService.createTimer(4, 19, 0);

    if (isPlatformBrowser(this.platformId)) {
      // Only start the timer if the user is viewing in the browser (I expect setTimeout or setInterval is being used internally, which breaks Angular Universal
      this.timer$
        .subscribe(td => {
          this.tickData = td;
        });
    } else {
      // Manually set once so the rendered page at least has an initial time
      this.tickData = this.countdownService.timerTick();
    }
  }
}
