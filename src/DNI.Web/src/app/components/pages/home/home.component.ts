import { Component, Inject, APP_ID, PLATFORM_ID, OnInit } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CountdownService } from 'app/services/countdown/countdown.service';
import { Observable } from 'rxjs';
import { TickData } from 'app/services/countdown/tick-data';
import { DataService } from 'app/services/creators/data.service';
import { Creator } from 'app/model/creator';

@Component({
  selector: 'dni-root',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  providers: [CountdownService]
})
export class HomeComponent implements OnInit {
  private timer$: Observable<TickData>;

  public tickData: TickData;

  constructor(
    @Inject(PLATFORM_ID)
    private platformId: Object,
    @Inject(APP_ID)
    private appId: string,
    private countdownService: CountdownService
  ) {
    console.log(this.appId + ' ' + this.platformId);
  }

  ngOnInit(): void {
    this.timer$ = this.countdownService.createTimer(4, 19, 0);
    this.timer$
      .subscribe(td => {
        this.tickData = td;
      });
  }

  public get title() {
    return '\'' + this.appId + '\' running in \'' + (isPlatformBrowser(this.platformId) ? 'browser' : 'server') + '\' mode';
  }
}
