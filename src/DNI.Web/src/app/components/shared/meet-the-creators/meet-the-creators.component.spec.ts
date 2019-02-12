import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { MeetTheCreatorsComponent } from './meet-the-creators.component';
import { LoadingComponent } from 'app/components/shared/loading/loading.component';

describe('MeetTheCreatorsComponent', () => {
  let component: MeetTheCreatorsComponent;
  let fixture: ComponentFixture<MeetTheCreatorsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule
      ],
      declarations: [
        MeetTheCreatorsComponent,
        LoadingComponent
      ]
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
