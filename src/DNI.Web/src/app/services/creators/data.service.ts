import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Creator } from 'app/model/creator';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  constructor(
    private http: HttpClient
  ) { }

  public creators(): Observable<Creator[]> {
    return this.http.get<Creator[]>('assets/data/creators.json');
  }
}
