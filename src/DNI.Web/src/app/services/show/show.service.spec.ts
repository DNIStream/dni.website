import { TestBed } from '@angular/core/testing';

import { ShowService } from './show.service';

let httpClientSpy: { get: jasmine.Spy };
let service: ShowService;

describe('ShowService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({});
    httpClientSpy = jasmine.createSpyObj('HttpClient', ['get']);
    service = new ShowService(<any>httpClientSpy);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
