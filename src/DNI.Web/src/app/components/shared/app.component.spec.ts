import { TestBed, async } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';
import { SocialLinksComponent } from 'app/components/shared/social-links/social-links.component';
import { CookieLawModule } from 'app/components/shared/angular2-cookie-law/angular2-cookie-law.module';

describe('AppComponent', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        CookieLawModule
      ],
      declarations: [
        AppComponent,
        SocialLinksComponent
      ],
    }).compileComponents();
  }));
  it('should create the app', async(() => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.debugElement.componentInstance;
    expect(app).toBeTruthy();
  }));
});
