import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Observable } from 'rxjs';
import { switchMap, map } from 'rxjs/operators';

import { ShowService } from 'app/services/show/show.service';
import { Show } from 'app/model/show';
import { SEOService } from 'app/services/seo/seo.service';

@Component({
  selector: 'dni-show-detail',
  templateUrl: './show-detail.component.html',
  styleUrls: ['./show-detail.component.scss']
})
export class ShowDetailComponent implements OnInit {

  public show$: Observable<Show>;

  private showData: Show;

  constructor(
    private route: ActivatedRoute,
    private showService: ShowService,
    private seoService: SEOService
  ) { }

  public ngOnInit(): void {
    this.showData = null;

    this.show$ = this.route
      .paramMap
      .pipe(
        switchMap((params: ParamMap) => {
          const slug = params.get('slug');
          return this.showService.getShow(slug);
        }),
        map(show => {
          this.showData = show;
          this.seoService.setTitle(show.title + ' | Documentation Not Included Podcast');
          this.seoService.setDescription(show.summary);
          return show;
        }));
  }
}
