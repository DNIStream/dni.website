import { TestBed } from '@angular/core/testing';

import { SEOService } from './seo.service';

describe('SEOService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: SEOService = TestBed.get(SEOService);
    expect(service).toBeTruthy();
  });

  it('#setTitle should prefix passed title to site name', () => {
    // Arrange
    const titleServiceSpy = jasmine.createSpyObj('Title', ['setTitle']);
    const metaServiceSpy = jasmine.createSpyObj('Meta', ['updateTag']);
    const service: SEOService = new SEOService(titleServiceSpy, metaServiceSpy);
    const expectedSuffix = ': DNI Stream Development Podcast';
    const expectedTitle = 'A page title';

    // Act
    service.setTitle(expectedTitle);

    // Assert
    expect(titleServiceSpy.setTitle)
      .toHaveBeenCalledWith(expectedTitle + expectedSuffix);
  });

  it('#setDescription should update description tag with passed description', () => {
    // Arrange
    const titleServiceSpy = jasmine.createSpyObj('Title', ['setTitle']);
    const metaServiceSpy = jasmine.createSpyObj('Meta', ['updateTag']);
    const service: SEOService = new SEOService(titleServiceSpy, metaServiceSpy);
    const expectedDescription = 'A meta description';

    // Act
    service.setDescription(expectedDescription);

    // Assert
    expect(metaServiceSpy.updateTag)
      .toHaveBeenCalledWith({ name: 'description', content: expectedDescription });
  });
});
