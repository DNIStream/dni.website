import { Component, OnInit, OnDestroy } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { switchMap, share } from 'rxjs/operators';

import { ShowService } from 'app/services/show/show.service';
import { SEOService } from 'app/services/seo/seo.service';
import { GetShowsRequest } from 'app/services/show/get-shows-request';
import { GetShowsResponse } from 'app/services/show/get-shows-response';

@Component({
  templateUrl: './show-archive.component.html',
  styleUrls: ['./show-archive.component.scss']
})
export class ShowArchiveComponent implements OnInit, OnDestroy {

  public getShowsResponse: GetShowsResponse = null;

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

  constructor(
    private showService: ShowService,
    private seoService: SEOService
  ) { }

  public ngOnInit(): void {
    this.seoService.setTitle('Documentation Not Included Podcast Episode Archive');
    this.seoService.setDescription('Documentation Not Included is DNI Stream\'s weekly live tech industry podcast that covers all aspects of the software industry, hosted by Chris and Josey. We feature guests from all over the globe with expertise in everything from development support and quality assurance, to technical and software architecture. Follow us on https://www.twitch.tv/dnistream to be notified when the show goes live.');

    // Set up initial paging and ordering values
    this.getShowsRequest$ = new BehaviorSubject<GetShowsRequest>(new GetShowsRequest());

    this.subscribeToShows();
  }

  public ngOnDestroy(): void {
    this.getShowsRequest$.unsubscribe();
  }

  public onSortSubmit(): void {
    this.refreshShows();
  }

  public onPaginationChanged(event: any): void {
    this.getShowsRequest$.value.currentPage = event.page;
    this.refreshShows();
  }

  private refreshShows(): void {
    // We null the .pagedShows property to ensure that the child pagers don't recursively call refreshShows()
    // This works with the hacky dni-loading and following <div> ngIfs and [hidden] logic due to a bug in the
    // pager
    this.getShowsResponse.pagedShows = null;
    // Emit the bound request value to the subscriber
    this.getShowsRequest$.next(this.getShowsRequest$.value);
  }

  private subscribeToShows(): void {
    this.getShowsRequest$
      .pipe(
        switchMap((getShowsRequest: GetShowsRequest) => this.showService.getShows(getShowsRequest)),
        share()
      )
      .subscribe(showResponse => this.getShowsResponse = showResponse);
  }
}
