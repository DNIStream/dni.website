import { ShowKeyword } from './../../model/show-keyword';
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { map, switchMap } from 'rxjs/operators';
import { Observable, of } from 'rxjs';

import { environment } from 'environments/environment';

import { Show } from 'app/model/show';
import { UriHelper } from 'app/components/shared/uriHelper';
import { ShowResponse } from 'app/model/show-response';
import { GetShowsRequest } from 'app/model/get-shows-request';

@Injectable({
  providedIn: 'root'
})
export class ShowService {
  constructor(
    private http: HttpClient
  ) { }

  public getShows(request: GetShowsRequest): Observable<ShowResponse> {
    let uri = environment.apiBaseUri + 'shows';

    uri = UriHelper.getUri(uri, {
      'sort-field': request.orderByField,
      'sort-order': request.orderByOrder,
      'page-num': request.currentPage.toString(),
      'items-per-page': request.itemsPerPage.toString()
    });

    return this.http
      .get<ShowResponse>(uri)
      .pipe(
        switchMap((response: ShowResponse) => {
          const newResponse = Object.assign(new ShowResponse(), response);
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
