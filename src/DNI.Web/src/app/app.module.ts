import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { NgLeftPadPipeModule } from 'angular-pipes';
import { NgxCaptchaModule } from 'ngx-captcha';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { PaginationModule } from 'ngx-bootstrap/pagination';

// Replace this import and delete the code once the fixes to the 3rd party component has been published to npm
import { CookieLawModule } from './components/shared/angular2-cookie-law/angular2-cookie-law.module';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './components/shared/app.component';
import { HomeComponent } from './components/pages/home/home.component';
import { PageNotFoundComponent } from './components/pages/page-not-found/page-not-found.component';
import { MeetTheCreatorsComponent } from './components/shared/meet-the-creators/meet-the-creators.component';
import { PrivacyComponent } from './components/pages/privacy/privacy.component';
import { ShowArchiveComponent } from './components/pages/show-archive/show-archive.component';
import { EthicsComponent } from './components/pages/ethics/ethics.component';
import { ContactComponent } from './components/pages/contact/contact.component';
import { FaqComponent } from './components/pages/faq/faq.component';
import { CommunityGuidelinesComponent } from './components/pages/community-guidelines/community-guidelines.component';
import { SocialLinksComponent } from './components/shared/social-links/social-links.component';
import { LoadingComponent } from './components/shared/loading/loading.component';
import { SafePipe } from './components/shared/safe-pipe/safe.pipe';
import { PagerComponent } from './components/shared/pager/pager.component';
import { ShowDetailComponent } from './components/pages/show-detail/show-detail.component';
import { KeywordTagsComponent } from './components/shared/keyword-tags/keyword-tags.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    PageNotFoundComponent,
    MeetTheCreatorsComponent,
    PrivacyComponent,
    ShowArchiveComponent,
    EthicsComponent,
    ContactComponent,
    FaqComponent,
    CommunityGuidelinesComponent,
    SocialLinksComponent,
    LoadingComponent,
    SafePipe,
    PagerComponent,
    ShowDetailComponent,
    KeywordTagsComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'dni' }),
    TabsModule.forRoot(),
    CollapseModule.forRoot(),
    PaginationModule.forRoot(),
    ReactiveFormsModule,
    FormsModule,
    NgxCaptchaModule,
    AppRoutingModule,
    NgLeftPadPipeModule,
    HttpClientModule,
    CookieLawModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
