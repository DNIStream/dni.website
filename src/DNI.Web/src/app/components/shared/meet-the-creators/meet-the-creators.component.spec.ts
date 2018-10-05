import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MeetTheCreatorsComponent } from './meet-the-creators.component';

describe('MeetTheCreatorsComponent', () => {
  let component: MeetTheCreatorsComponent;
  let fixture: ComponentFixture<MeetTheCreatorsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MeetTheCreatorsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MeetTheCreatorsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
