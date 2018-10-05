import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowArchiveComponent } from './show-archive.component';

describe('ShowArchiveComponent', () => {
  let component: ShowArchiveComponent;
  let fixture: ComponentFixture<ShowArchiveComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShowArchiveComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowArchiveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
