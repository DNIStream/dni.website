import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';

import { Observable } from 'rxjs';
import { switchMap, map } from 'rxjs/operators';

import { Show } from '../../../model/show';
import { ShowService } from '../../../services/show/show.service';
import { SEOService } from '../../../services/seo/seo.service';


@Component({
  selector: 'dni-show-detail',
  templateUrl: './show-detail.component.html',
  styleUrls: ['./show-detail.component.scss']
})
export class ShowDetailComponent implements OnInit {

  public show$: Observable<Show>;

  constructor(
    private route: ActivatedRoute,
    private showService: ShowService,
    private seoService: SEOService
  ) { }

  public ngOnInit(): void {
    this.show$ = this.route
      .paramMap
      .pipe(
        switchMap((params: ParamMap) => {
          const slug = params.get('slug');
          return this.showService.getShow(slug);
        }),
        map(show => {
          this.seoService.setTitle(show.title + ' | Documentation Not Included Podcast');
          this.seoService.setDescription(show.summary);
          return show;
        }));
  }
}
