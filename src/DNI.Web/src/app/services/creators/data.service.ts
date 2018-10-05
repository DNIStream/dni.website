import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Creator } from 'app/model/creator';
import { environment } from 'environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  constructor(
    private http: HttpClient
  ) { }

  public creators(): Observable<Creator[]> {
    return this.http.get<Creator[]>(environment.webUri + 'assets/data/creators.json');
  }
}
