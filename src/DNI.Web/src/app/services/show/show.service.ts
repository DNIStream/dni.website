import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, switchMap } from 'rxjs/operators';
import { Observable, of } from 'rxjs';

import { environment } from 'environments/environment';

import { Show } from 'app/model/show';
import { UriHelper } from 'app/components/shared/uriHelper';
import { GetShowsRequest } from 'app/services/show/get-shows-request';
import { GetShowsResponse } from './get-shows-response';

@Injectable({
  providedIn: 'root'
})
export class ShowService {
  constructor(
    private http: HttpClient
  ) { }

  public getShows(request: GetShowsRequest): Observable<GetShowsResponse> {
    let uri = environment.apiBaseUri + 'shows';

    uri = UriHelper.getUri(uri, {
      'sort-field': request.orderByField,
      'sort-order': request.orderByOrder,
      'page-num': request.currentPage.toString(),
      'items-per-page': request.itemsPerPage.toString()
    });

    return this.http
      .get<GetShowsResponse>(uri)
      .pipe(
        switchMap((response: GetShowsResponse) => {
          const newResponse = Object.assign(new GetShowsResponse(), response);
          newResponse.pagedShows = response.pagedShows.map(s => Object.assign(new Show(), s));
          newResponse.latestShow = Object.assign(new Show(), response.latestShow);
          return of(newResponse);
        }));
  }

  public getShow(slug: string): Observable<Show> {
    const uri = environment.apiBaseUri + 'show/' + slug;
    return this.http
      .get<Show>(uri)
      .pipe(map((response: Show) => Object.assign(new Show(), response)));
  }
}
