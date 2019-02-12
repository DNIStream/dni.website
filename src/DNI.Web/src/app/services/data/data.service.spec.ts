import { TestBed } from '@angular/core/testing';

import { DataService } from 'app/services/data/data.service';

let httpClientSpy: { get: jasmine.Spy };
let service: DataService;

describe('DataService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({});
    httpClientSpy = jasmine.createSpyObj('HttpClient', ['get']);
    service = new DataService(<any>httpClientSpy);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
