import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { ShowService } from 'app/services/show/show.service';
import { SEOService } from 'app/services/seo/seo.service';
import { Show } from 'app/model/show';
import { ShowResponse } from 'app/model/show-response';


@Component({
  templateUrl: './show-archive.component.html',
  styleUrls: ['./show-archive.component.scss']
})
export class ShowArchiveComponent implements OnInit, OnDestroy {

  public shows: Observable<Show[]>;

  public filterFields: { [key: string]: string; } = {
    'PublishedTime': 'Date',
    'Title': 'Episode Title',
    'DurationInSeconds': 'Length'
  };

  public filterOrders: string[] = [
    'Descending',
    'Ascending'
  ];

  public filters = {
    orderByField: Object.keys(this.filterFields)[0],
    orderByOrder: this.filterOrders[0]
  };

  //#region Paging info

  public itemsPerPage: number;
  public currentPage: number = 1;
  public totalItems: number;
  public totalPages: number;

  //#endregion

  private _filterKeys: string[];
  public get filterKeys(): string[] {
    if (!this._filterKeys) {
      this._filterKeys = Object.keys(this.filterFields);
    }
    return this._filterKeys;
  }

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

  public getFilterName(key: string) {
    return this.filterFields[key];
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

    this.shows = this.showService
      .getShows(this.filters.orderByField, this.filters.orderByOrder, this.currentPage, this.itemsPerPage)
      .pipe(
        switchMap((showResponse: ShowResponse) => {
          this.totalItems = showResponse.pageInfo.totalRecords;
          this.itemsPerPage = showResponse.pageInfo.itemsPerPage;
          this.totalPages = showResponse.pageInfo.totalPages;
          return of(showResponse.pagedShows);
        }));
  }
}
