import { ShowResponse } from 'app/model/show-response';
import { Component, OnInit, OnDestroy } from '@angular/core';

import { ShowService } from 'app/services/show/show.service';
import { SEOService } from 'app/services/seo/seo.service';
import { Show } from 'app/model/show';

@Component({
  templateUrl: './show-archive.component.html',
  styleUrls: ['./show-archive.component.scss']
})
export class ShowArchiveComponent implements OnInit, OnDestroy {

  public shows: Show[];

  public filterFields: string[] = [
    'PublishedTime',
    'Title',
    'DurationInSeconds'
  ];

  public filterOrders: string[] = [
    'Descending',
    'Ascending'
  ];

  public filters = {
    orderByField: this.filterFields[0],
    orderByOrder: this.filterOrders[0]
  };

  //#region Paging info

  private itemsPerPage: number;
  private currentPage: number = 1;
  private totalItems: number;
  private totalPages: number;

  //#endregion

  constructor(
    private showService: ShowService,
    private seoService: SEOService
  ) {
    this.shows = null;
  }

  ngOnInit() {
    this.seoService.setTitle('Podcast Archive');
    this.seoService.setDescription('The top-level list of our podcast and vodcast shows');

    this.refreshShows();
  }

  ngOnDestroy(): void {
    this.shows = null;
  }

  public onSubmit(): void {
    this.refreshShows();
  }

  public onPageChanged(event: any): void {
    this.currentPage = event.page;
    this.refreshShows();
  }

  private refreshShows(): void {
    this.shows = null;

    this.showService
      .getShows(this.filters.orderByField, this.filters.orderByOrder, this.currentPage, this.itemsPerPage)
      .subscribe({
        next: (response: ShowResponse) => {
          this.shows = response.pagedShows;
          this.totalItems = response.pageInfo.totalRecords;
          this.itemsPerPage = response.pageInfo.itemsPerPage;
          this.totalPages = response.pageInfo.totalPages;
        },
        error: (e: any) => {
          this.shows = null;
        }
      });
  }
}
