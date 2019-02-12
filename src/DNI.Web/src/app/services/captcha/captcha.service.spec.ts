import { TestBed } from '@angular/core/testing';

import { CaptchaService } from './captcha.service';

let httpClientSpy: { get: jasmine.Spy };
let service: CaptchaService;

describe('CaptchaService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({});
    httpClientSpy = jasmine.createSpyObj('HttpClient', ['get']);
    service = new CaptchaService(<any>httpClientSpy);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
