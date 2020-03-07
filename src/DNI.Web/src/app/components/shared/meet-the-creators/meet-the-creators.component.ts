import { Component, OnInit } from '@angular/core';
import { of, Observable } from 'rxjs';
import { switchMap, map, share } from 'rxjs/operators';

import { DataService } from 'app/services/data/data.service';
import { Creator } from 'app/model/creator';


@Component({
  selector: 'dni-meet-the-creators',
  templateUrl: './meet-the-creators.component.html',
  styleUrls: ['./meet-the-creators.component.scss']
})
export class MeetTheCreatorsComponent implements OnInit {

  public creators$: Observable<Creator[]>;

  constructor(
    private dataService: DataService
  ) { }

  public ngOnInit(): void {
    this.creators$ = this.dataService
      .getCreators()
      .pipe(
        map((creators: Creator[]) => creators.sort(() => .5 - Math.random()))
      );
  }
}
