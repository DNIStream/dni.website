import { Component, OnInit } from '@angular/core';

import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { Creator } from '../../../model/creator';
import { DataService } from '../../../services/data/data.service';

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
