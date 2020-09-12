import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CaptchaService {
  constructor(
    protected http: HttpClient
  ) { }

  public verify(userResponse): Observable<HttpResponse<Object>> {
    const uri = environment.apiBaseUri + 'captcha';
    const body = {
      'UserResponse': userResponse
    };

    return this.http.post(uri, body, { observe: 'response' });
  }
}
