import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';

import { ContactModel } from 'app/services/contact/contact-model';
import { environment } from 'environments/environment';

@Injectable()
export class ContactService {
  constructor(
    private http: HttpClient
  ) { }

  public sendContactEmail(model: ContactModel): Observable<HttpResponse<Object>> {
    const uri = environment.apiBaseUri + 'contact';
    return this.http.post(uri, model, { observe: 'response' });
  }
}
