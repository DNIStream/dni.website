import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { Show } from 'app/model/show';
import { environment } from 'environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ShowService {

  constructor(
    private http: HttpClient
  ) { }
}
