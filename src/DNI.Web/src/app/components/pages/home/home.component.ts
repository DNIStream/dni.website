import { Component, OnInit } from '@angular/core';
import { CountdownService } from 'app/services/countdown/countdown.service';
import { Observable } from 'rxjs';
import { TickData } from 'app/services/countdown/tick-data';

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
    private countdownService: CountdownService
  ) {
  }

  ngOnInit(): void {
    this.timer$ = this.countdownService.createTimer(4, 19, 0);
    this.timer$
      .subscribe(td => {
        this.tickData = td;
      });
  }
}
