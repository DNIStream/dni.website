import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from 'environments/environment';

import { Show } from 'app/model/show';

@Injectable({
  providedIn: 'root'
})
export class ShowService {
  constructor(
    private http: HttpClient
  ) { }

  public getShows(): Observable<Show[]> {
    const uri = environment.apiBaseUri + 'shows';

    return this.http
      .get<Show[]>(uri)
      .pipe(map(shows => {
        return shows.map(s => Object.assign(new Show(), s));
      }));
  }
}
