import { TestBed } from '@angular/core/testing';

import { SEOService } from './seo.service';

describe('SEOService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: SEOService = TestBed.get(SEOService);
    expect(service).toBeTruthy();
  });

  it('#setTitle should prefix passed title to site name', () => {
    const titleServiceSpy = jasmine.createSpyObj('Title', ['setTitle']);
    const service: SEOService = new SEOService(titleServiceSpy);
    const expectedSuffix = ': Documentation Not Included Development Podcast';
    const expectedTitle = 'A page title';

    service.setTitle(expectedTitle);

    expect(titleServiceSpy.setTitle)
      .toHaveBeenCalledWith(expectedTitle + expectedSuffix);
  });
});
