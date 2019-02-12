import { TestBed, async } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { LeftPadPipe } from 'angular-pipes';
import { CookieLawModule } from 'app/components/shared/angular2-cookie-law/angular2-cookie-law.module';

import { HomeComponent } from './home.component';
import { SocialLinksComponent } from 'app/components/shared/social-links/social-links.component';
import { MeetTheCreatorsComponent } from 'app/components/shared/meet-the-creators/meet-the-creators.component';
import { LoadingComponent } from 'app/components/shared/loading/loading.component';

describe('HomeComponent', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        HttpClientTestingModule,
        CookieLawModule
      ],
      declarations: [
        HomeComponent,
        SocialLinksComponent,
        LeftPadPipe,
        MeetTheCreatorsComponent,
        LoadingComponent
      ],
    }).compileComponents();
  }));

  it('should create component', async(() => {
    const fixture = TestBed.createComponent(HomeComponent);
    const app = fixture.debugElement.componentInstance;
    expect(app).toBeTruthy();
  }));

  // it(`should inject appId`, async(() => {
  //   const fixture = TestBed.createComponent(HomeComponent);
  //   const app = fixture.debugElement.componentInstance;
  //   expect(app.).toContain('dni');
  // }));
  // it('should render title in a h1 tag', async(() => {
  //   const fixture = TestBed.createComponent(HomeComponent);
  //   fixture.detectChanges();
  //   const compiled = fixture.debugElement.nativeElement;
  //   expect(compiled.querySelector('h1').textContent).toContain('Welcome to \'dni\' running in \'browser\' mode!');
  // }));
});
