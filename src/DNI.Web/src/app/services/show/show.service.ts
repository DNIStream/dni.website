import { map, switchMap, flatMap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from 'environments/environment';

import { Show } from 'app/model/show';
import { UriHelper } from 'app/components/shared/uriHelper';
import { ShowResponse } from 'app/model/show-response';

@Injectable({
  providedIn: 'root'
})
export class ShowService {
  constructor(
    private http: HttpClient
  ) { }

  public getShows(orderByField: string, orderByOrder: string, pageNo: number = 1, itemsPerPage: number = 7): Observable<ShowResponse> {
    let uri = environment.apiBaseUri + 'shows';

    uri = UriHelper.getUri(uri, {
      'sort-field': orderByField,
      'sort-order': orderByOrder,
      'page-num': pageNo.toString(),
      'items-per-page': itemsPerPage.toString()
    });

    return this.http
      .get<ShowResponse>(uri)
      .pipe(
        map((response: ShowResponse) => {
          response.pagedShows = response.pagedShows.map(s => Object.assign(new Show(), s));
          response.latestShow = Object.assign(new Show(), response.latestShow);
          return response;
        }));

  }
}
