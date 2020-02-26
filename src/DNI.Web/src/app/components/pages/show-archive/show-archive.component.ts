import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable, of } from 'rxjs';
import { switchMap, share } from 'rxjs/operators';

import { ShowService } from 'app/services/show/show.service';
import { SEOService } from 'app/services/seo/seo.service';
import { Show } from 'app/model/show';
import { ShowResponse } from 'app/model/show-response';
import { ShowKeyword } from 'app/model/show-keyword';
import { GetShowsRequest } from 'app/model/get-shows-request';

@Component({
  templateUrl: './show-archive.component.html',
  styleUrls: ['./show-archive.component.scss']
})
export class ShowArchiveComponent implements OnInit {

  public shows$: Observable<Show[]> = null;

  public getShowsRequest$: Observable<GetShowsRequest>;

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
  public globalKeywords: ShowKeyword[];

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
  ) { }

  ngOnInit() {
    this.seoService.setTitle('Documentation Not Included Podcast Episode Archive');
    this.seoService.setDescription('Documentation Not Included is DNI Stream\'s weekly live tech industry podcast that covers all aspects of the software industry, hosted by Chris and Josey. We feature guests from all over the globe with expertise in everything from development support and quality assurance, to technical and software architecture. Follow us on https://www.twitch.tv/dnistream to be notified when the show goes live.');

    console.log('init');
    // this.subscribeToShows();

    this.getShowsRequest$ = new Observable<GetShowsRequest>();
  }

  public getFilterName(key: string) {
    return this.filterFields[key];
  }

  public onSubmit(): void {
    console.log('onSubmit');

    // this.refreshShows();
  }

  public onPageChanged(event: any): void {
    console.log('onPageChanged');
    this.currentPage = event.page;

    // this.refreshShows();
  }

  private subscribeToShows(): void {
    console.log('refreshShows()');

    this.shows$ = this.showService
      .getShows(this.filters.orderByField, this.filters.orderByOrder, this.currentPage, this.itemsPerPage)
      .pipe(
        switchMap((showResponse: ShowResponse) => {
          console.log('switchMap() entry');
          this.totalItems = showResponse.pageInfo.totalRecords;
          this.itemsPerPage = showResponse.pageInfo.itemsPerPage;
          this.totalPages = showResponse.pageInfo.totalPages;
          this.globalKeywords = showResponse
            .getKeywords()
            .sort((a, b) => {
              console.log('sort()');
              const countSort = (a.count * -1) - (b.count * -1);
              let keywordSort: number;
              if (a.keyword < b.keyword) {
                keywordSort = -1;
              } else if (a.keyword > b.keyword) {
                keywordSort = 1;
              } else {
                keywordSort = 0;
              }

              return countSort || keywordSort;
            })
            .slice(0, 10);
          console.log('switchMap() exit');
          return of(showResponse.pagedShows);
        }),
        share());
  }
}
