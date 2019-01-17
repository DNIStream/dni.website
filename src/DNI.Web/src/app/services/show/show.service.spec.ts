import { TestBed } from '@angular/core/testing';

import { ShowService } from './show.service';

describe('ShowService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ShowService = TestBed.get(ShowService);
    expect(service).toBeTruthy();
  });
});
