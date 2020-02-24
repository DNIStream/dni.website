import { HttpResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Observable, of } from 'rxjs';
import { switchMap, map } from 'rxjs/operators';

import { ShowService } from 'app/services/show/show.service';
import { Show } from 'app/model/show';

@Component({
  selector: 'dni-show-detail',
  templateUrl: './show-detail.component.html',
  styleUrls: ['./show-detail.component.scss']
})
export class ShowDetailComponent implements OnInit {

  public show: Observable<Show>;

  private showData: Show;

  constructor(
    private route: ActivatedRoute,
    private showService: ShowService
  ) { }

  public ngOnInit(): void {
    this.showData = null;

    this.show = this.route
      .paramMap
      .pipe(
        switchMap((params: ParamMap) => {
          const slug = params.get('slug');
          return this.showService.getShow(slug);
        }),
        map(show => {
          this.showData = show;
          return show;
        }));
  }
}
