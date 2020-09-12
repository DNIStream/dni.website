import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { map, switchMap } from 'rxjs/operators';
import { Observable, of } from 'rxjs';

import { GetShowsResponse } from './get-shows-response';
import { GetShowsRequest } from './get-shows-request';
import { environment } from '../../../environments/environment';
import { UriHelper } from '../../components/shared/uriHelper';
import { Show } from '../../model/show';

@Injectable({
  providedIn: 'root'
})
export class ShowService {
  constructor(
    private http: HttpClient
  ) { }

  public getShows(request: GetShowsRequest): Observable<GetShowsResponse> {
    let uri = environment.apiBaseUri + (!request.keyword ? 'shows' : `shows/${request.keyword}`);

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
