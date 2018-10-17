import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CookieLawModule } from 'angular2-cookie-law';
import { NgStringPipesModule } from 'angular-pipes';
import { NgxCaptchaModule } from 'ngx-captcha';
import { TabsModule } from 'ngx-bootstrap';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './components/shared/app.component';
import { HomeComponent } from 'app/components/pages/home/home.component';
import { PageNotFoundComponent } from 'app/components/pages/page-not-found/page-not-found.component';
import { MeetTheCreatorsComponent } from 'app/components/shared/meet-the-creators/meet-the-creators.component';
import { PrivacyComponent } from './components/pages/privacy/privacy.component';
import { ShowArchiveComponent } from './components/pages/show-archive/show-archive.component';
import { EthicsComponent } from './components/pages/ethics/ethics.component';
import { ContactComponent } from './components/pages/contact/contact.component';
import { FaqComponent } from './components/pages/faq/faq.component';
import { CommunityGuidelinesComponent } from './components/pages/community-guidelines/community-guidelines.component';
import { SocialLinksComponent } from './components/shared/social-links/social-links.component';
import { LoadingComponent } from './components/shared/loading/loading.component';

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
    LoadingComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'dni' }),
    TabsModule.forRoot(),
    ReactiveFormsModule,
    FormsModule,
    NgxCaptchaModule,
    AppRoutingModule,
    NgStringPipesModule,
    HttpClientModule,
    BrowserAnimationsModule,
    CookieLawModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
