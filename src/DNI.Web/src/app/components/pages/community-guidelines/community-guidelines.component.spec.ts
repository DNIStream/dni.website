import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CommunityGuidelinesComponent } from './community-guidelines.component';
import { RouterTestingModule } from '@angular/router/testing';

describe('CommunityGuidelinesComponent', () => {
  let component: CommunityGuidelinesComponent;
  let fixture: ComponentFixture<CommunityGuidelinesComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [CommunityGuidelinesComponent],
      imports: [
        RouterTestingModule
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommunityGuidelinesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
