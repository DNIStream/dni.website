import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable, of, BehaviorSubject } from 'rxjs';
import { switchMap, share } from 'rxjs/operators';

import { ShowService } from 'app/services/show/show.service';
import { SEOService } from 'app/services/seo/seo.service';
import { ShowKeyword } from 'app/model/show-keyword';
import { GetShowsRequest } from 'app/services/show/get-shows-request';
import { GetShowsResponse } from 'app/services/show/get-shows-response';

@Component({
  templateUrl: './show-archive.component.html',
  styleUrls: ['./show-archive.component.scss']
})
export class ShowArchiveComponent implements OnInit, OnDestroy {

  public getShowsResponse$: Observable<GetShowsResponse> = null;

  public getShowsRequest$: BehaviorSubject<GetShowsRequest>;

  public sortFieldOptions: { [key: string]: string; } = {
    'PublishedTime': 'Date',
    'Title': 'Episode Title',
    'DurationInSeconds': 'Length'
  };

  private _sortFieldOptionKeys: string[];
  public get sortFieldOptionKeys(): string[] {
    if (!this._sortFieldOptionKeys) {
      this._sortFieldOptionKeys = Object.keys(this.sortFieldOptions);
    }
    return this._sortFieldOptionKeys;
  }

  public sortFieldOrders: string[] = [
    'Descending',
    'Ascending'
  ];

  //#region Paging info

  // public itemsPerPage: number;
  // public currentPage: number = 1;
  public totalItems: number;
  public totalPages: number;
  public globalKeywords: ShowKeyword[];

  //#endregion


  constructor(
    private showService: ShowService,
    private seoService: SEOService
  ) { }

  public ngOnInit(): void {
    this.seoService.setTitle('Documentation Not Included Podcast Episode Archive');
    this.seoService.setDescription('Documentation Not Included is DNI Stream\'s weekly live tech industry podcast that covers all aspects of the software industry, hosted by Chris and Josey. We feature guests from all over the globe with expertise in everything from development support and quality assurance, to technical and software architecture. Follow us on https://www.twitch.tv/dnistream to be notified when the show goes live.');

    console.log('init');

    // Set up initial paging and ordering values
    this.getShowsRequest$ = new BehaviorSubject<GetShowsRequest>(new GetShowsRequest());

    this.subscribeToShows();
  }

  public ngOnDestroy(): void {
    console.log('destroy');
    this.getShowsRequest$.unsubscribe();
  }

  public onSortSubmit(): void {
    console.log('onSubmit');

    this.refreshShows();
  }

  public onPaginationChanged(event: any): void {
    console.log('onPaginationChanged');
    // this.currentPage = event.page;
    this.getShowsRequest$.value.currentPage = event.page;
    this.refreshShows();
  }

  private refreshShows(): void {
    // Emit the bound request value to the subscriber
    this.getShowsRequest$.next(this.getShowsRequest$.value);
  }

  private subscribeToShows(): void {
    console.log('subscribeToShows()');

    this.getShowsRequest$
      .pipe(share())
      .subscribe(getShowsRequest => {
        console.log('subscription call');
        this.getShowsResponse$ = this.showService
          .getShows(getShowsRequest)
          .pipe(
            switchMap((showResponse: GetShowsResponse) => {
              console.log('switchMap() entry');
              this.totalItems = showResponse.pageInfo.totalRecords;
              // this.itemsPerPage = showResponse.pageInfo.itemsPerPage;
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
              return of(showResponse);
            }),
            share()
          );
      });
  }
}
