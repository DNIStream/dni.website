import { Component, OnInit } from '@angular/core';
import { interval, Observable } from 'rxjs';

import * as moment from 'moment';

@Component({
  selector: 'dni-countdown',
  templateUrl: './countdown.component.html',
  styleUrls: ['./countdown.component.scss']
})
export class CountdownComponent implements OnInit {
  public timeRemaining: string;

  private $timer: Observable<number> = interval(1000);

  private nextShowDate: Date = new Date(2018, 9, 9, 19, 0);

  constructor() { }

  ngOnInit() {
    this.setupTimer();
  }

  private setupTimer() {
    this.$timer
      .subscribe(x => {
        this.timeRemaining = moment()
          .diff(this.nextShowDate, 'seconds')
          .toString();
        // this.timeRemaining = moment(this.nextShowDate)
        //   .fromNow();
        console.log('tock');
      });
  }
}
