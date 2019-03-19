import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from 'environments/environment';

import { Show } from 'app/model/show';
import { UriHelper } from 'app/components/shared/uriHelper';

@Injectable({
  providedIn: 'root'
})
export class ShowService {
  constructor(
    private http: HttpClient
  ) { }

  public getShows(orderByField: string, orderByOrder: string): Observable<Show[]> {
    let uri = environment.apiBaseUri + 'shows';

    uri = UriHelper.getUri(uri, {
      orderByField: orderByField,
      orderByOrder: orderByOrder
    });

    return this.http
      .get<Show[]>(uri)
      .pipe(map(shows => {
        return shows.map(s => Object.assign(new Show(), s));
      }));
  }
}
