import { TestBed } from '@angular/core/testing';

import { ContactService } from './contact.service';

let httpClientSpy: { get: jasmine.Spy };
let service: ContactService;

describe('ContactService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({});
    httpClientSpy = jasmine.createSpyObj('HttpClient', ['get']);
    service = new ContactService(<any>httpClientSpy);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
