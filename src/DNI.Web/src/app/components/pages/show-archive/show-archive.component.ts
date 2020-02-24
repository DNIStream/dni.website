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
    this.seoService.setTitle('Documentation Not Included Podcast Episode Archive');
    this.seoService.setDescription('Documentation Not Included is DNI Stream\'s weekly live tech industry podcast that covers all aspects of the software industry, hosted by Chris and Josey. We feature guests from all over the globe with expertise in everything from development support and quality assurance, to technical and software architecture. Follow us on https://www.twitch.tv/dnistream to be notified when the show goes live.');

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
