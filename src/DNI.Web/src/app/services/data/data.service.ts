import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Creator } from 'app/model/creator';
import { environment } from 'environments/environment';
import { DiscoveryPlatform } from 'app/model/discovery-platform';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  constructor(
    private http: HttpClient
  ) { }

  public creators(): Observable<Creator[]> {
    const uri = environment.webUri + 'assets/data/creators.json';
    return this.http.get<Creator[]>(uri);
  }

  public discoveryPlatforms(): Observable<DiscoveryPlatform[]> {
    const uri = environment.webUri + 'assets/data/discovery-platforms.json';
    return this.http.get<DiscoveryPlatform[]>(uri);
  }
}
