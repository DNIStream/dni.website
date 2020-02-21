import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Observable } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { ShowService } from 'app/services/show/show.service';
import { Show } from 'app/model/show';

@Component({
  selector: 'dni-show-detail',
  templateUrl: './show-detail.component.html',
  styleUrls: ['./show-detail.component.scss']
})
export class ShowDetailComponent implements OnInit {

  private show: Observable<Show>;

  constructor(
    private route: ActivatedRoute,
    private showService: ShowService
  ) { }

  public ngOnInit(): void {
    this.show = this.route
      .paramMap
      .pipe(
        switchMap((params: ParamMap) => {
          const slug = params.get('slug');
          console.log(slug);
          return this.showService.getShow(slug);
        }));
  }
}
