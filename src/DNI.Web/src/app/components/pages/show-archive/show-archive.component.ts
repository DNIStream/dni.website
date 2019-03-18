import { Component, OnInit, OnDestroy } from '@angular/core';

import { ShowService } from 'app/services/show/show.service';
import { Show } from 'app/model/show';
import { SEOService } from 'app/services/seo/seo.service';

@Component({
  templateUrl: './show-archive.component.html',
  styleUrls: ['./show-archive.component.scss']
})
export class ShowArchiveComponent implements OnInit, OnDestroy {

  public model: Show[];

  public filterFields: string[] = [
    'PublishedTime',
    'Version'
  ];

  public filterOrders: string[] = [
    'Descending',
    'Ascending'
  ];

  public filters = {
    orderByField: this.filterFields[0],
    orderByOrder: this.filterOrders[0]
  };

  constructor(
    private showService: ShowService,
    private seoService: SEOService
  ) {
    this.model = null;
  }

  ngOnInit() {
    this.seoService.setTitle('Podcast Archive');
    this.seoService.setDescription('The top-level list of our podcast and vodcast shows');

    this.refreshShows();
  }

  ngOnDestroy(): void {
    this.model = null;
  }

  public onSubmit(): void {
    this.refreshShows();
  }

  private refreshShows(): void {
    this.model = null;

    this.showService
      .getShows(this.filters.orderByField, this.filters.orderByOrder)
      .subscribe(shows => this.model = shows,
        e => {
          this.model = null;
        });
  }
}
