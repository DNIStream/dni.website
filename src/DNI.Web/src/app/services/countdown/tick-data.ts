import * as moment from 'moment';

export class TickData {
    constructor(
        /**
     * Boolean updated each tick to inform parent components that the timer has
     * expired within the the last showDurationHours.
     */
        public showIsLive: boolean,

        /**
         * The amount of time until the next show
         */
        public nextShowIn: moment.Duration,

        /**
         * The date of the next show as a Moment object.
         */
        public nextShowDate: moment.Moment,

        /**
       * The date of the last show as a Moment object.
       */
        public lastShowDate: moment.Moment,
    ) { }
}
