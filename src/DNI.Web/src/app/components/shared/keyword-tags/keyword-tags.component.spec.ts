import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { KeywordTagsComponent } from './keyword-tags.component';

describe('KeywordTagsComponent', () => {
  let component: KeywordTagsComponent;
  let fixture: ComponentFixture<KeywordTagsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ KeywordTagsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(KeywordTagsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
