import { Injectable } from '@angular/core';
import { Observable, interval, timer } from 'rxjs';
import { map } from 'rxjs/operators';
import * as moment from 'moment-timezone';
import { TickData } from 'app/services/countdown/tick-data';

@Injectable()
export class CountdownService {

  //#region Private fields

  private _timer$: Observable<number>;
  private _nextShowIn: moment.Duration;
  private _nextShowDate: moment.Moment;
  private _lastShowDate: moment.Moment;
  private _showIsLive: boolean = false;

  // Init backing fields
  private _showDay: string | number;
  private _showHour: number;
  private _showMinutes: number;
  private _showDurationHours: number;
  private _showRunOverHours: number;

  //#endregion

  constructor() { }

  /**
   * Creates an interval() observable that ticks once per second, populating
   * @param showDay The day part of the show schedule. Pass 'Thurs', 'Thursday' or '4'
   * @param showHour The hour part of the show schedule. 24 Hour format, 0 - 23. E.g. 19 is 7PM GMT / BST.
   * @param showMinutes The minute part of the show schedule. 0 - 59.
   * @param showDurationHours The amount of time the show is LIVE from the specified showDay,showHour and showMinutes. E.g. 1.5 = 1 hour 30 minutes.
   * @param showRunOverHours The amount of additional time (in hours) that the show will be considered "live" after the showDurationHours. E.g. 0.25 = 15 minutes.
   */
  public createTimer(showDay: string | number = 0, showHour: number = 19, showMinutes: number = 0,
    showDurationHours: number = 1, showRunOverHours: number = .25): Observable<TickData> {
    this._showDay = showDay;
    this._showHour = showHour;
    this._showMinutes = showMinutes;
    this._showDurationHours = showDurationHours;
    this._showRunOverHours = showRunOverHours;

    this.calculateShowDates();
    this.timerTick(); // Initial tick to show timer on init

    return timer(0, 1000)
      .pipe(map(() => {
        return this.timerTick();
      }));
  }

  private calculateShowDates() {
    this.calculateLastShowDate();
    this.calculateNextShowDate();
  }

  /**
   * Discovers the next show date based on the GMT / BST settings passed into the component.
   */
  private calculateNextShowDate() {
    this._nextShowDate = this.getShowDate();
    if (this._nextShowDate <= moment()) {
      this._nextShowDate = this._nextShowDate.add(1, 'week');
    }
  }

  /**
   * Discovers the last show date based on the GMT / BST settings passed into the component.
   */
  private calculateLastShowDate() {
    this._lastShowDate = this.getShowDate();
    if (this._lastShowDate > moment()) {
      this._lastShowDate = this._lastShowDate.add(-1, 'week');
    }
  }

  private getShowDate(): moment.Moment {
    return moment()
      .tz('Europe/London')
      .day(this._showDay)
      .hour(this._showHour)
      .minute(this._showMinutes)
      .second(0);
  }

  /**
   * Ticks the timer, calculating the time until the next show,
   * and sets the showIsLive property based on the showDurationHours input
   */
  private timerTick(): TickData {
    this.calculateIsLive();
    this.calculateTimeUntilNextShow();
    return new TickData(this._showIsLive, this._nextShowIn, this._nextShowDate, this._lastShowDate);
  }

  private calculateIsLive() {
    const diffFromLastShow = moment().valueOf() - this._lastShowDate.valueOf();
    const durationFromLastShow = moment.duration(diffFromLastShow);
    this._showIsLive = durationFromLastShow.asHours() <= (this._showDurationHours + this._showRunOverHours);
  }

  private calculateTimeUntilNextShow() {
    const diffToNextShow = this._nextShowDate.valueOf() - moment().valueOf();
    if (diffToNextShow <= 0) {
      this.calculateShowDates();
      return;
    }
    this._nextShowIn = moment.duration(diffToNextShow);
  }
}
