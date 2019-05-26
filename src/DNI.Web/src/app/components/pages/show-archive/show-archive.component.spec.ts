import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';

import { CollapseModule } from 'ngx-bootstrap';

import { ShowArchiveComponent } from './show-archive.component';
import { SafePipe } from 'app/components/shared/safe-pipe/safe.pipe';
import { LoadingComponent } from 'app/components/shared/loading/loading.component';

describe('ShowArchiveComponent', () => {
  let component: ShowArchiveComponent;
  let fixture: ComponentFixture<ShowArchiveComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        ShowArchiveComponent,
        LoadingComponent,
        SafePipe
      ],
      imports: [
        RouterTestingModule,
        CollapseModule,
        FormsModule,
        HttpClientTestingModule
      ]
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
