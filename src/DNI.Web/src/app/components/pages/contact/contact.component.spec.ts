import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';

import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxCaptchaModule } from 'ngx-captcha';

import { ContactComponent } from './contact.component';
import { LoadingComponent } from 'app/components/shared/loading/loading.component';
import { SocialLinksComponent } from 'app/components/shared/social-links/social-links.component';

describe('ContactComponent', () => {
  let component: ContactComponent;
  let fixture: ComponentFixture<ContactComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        ContactComponent,
        LoadingComponent,
        SocialLinksComponent
      ],
      imports: [
        RouterTestingModule,
        TabsModule.forRoot(),
        FormsModule,
        NgxCaptchaModule,
        HttpClientTestingModule
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContactComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
