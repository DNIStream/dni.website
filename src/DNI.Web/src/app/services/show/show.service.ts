import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Show } from 'app/model/show';

@Injectable({
  providedIn: 'root'
})
export class ShowService {

  constructor(
    private http: HttpClient
  ) { }

  public getShows(): Show[] {
    return null;
  }
}
