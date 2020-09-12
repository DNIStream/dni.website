import { TestBed } from '@angular/core/testing';

import { CountdownService } from './countdown.service';

describe('CountdownService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CountdownService = TestBed.inject(CountdownService);
    expect(service).toBeTruthy();
  });
});
