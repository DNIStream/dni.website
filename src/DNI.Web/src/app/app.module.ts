import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { NgStringPipesModule } from 'angular-pipes';
import { CookieLawModule } from 'angular2-cookie-law';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from 'app/app.component';
import { HomeComponent } from 'app/components/pages/home/home.component';
import { PageNotFoundComponent } from 'app/components/pages/page-not-found/page-not-found.component';
import { MeetTheCreatorsComponent } from 'app/components/shared/meet-the-creators/meet-the-creators.component';
import { PrivacyComponent } from './components/pages/privacy/privacy.component';
import { ShowArchiveComponent } from './components/pages/show-archive/show-archive.component';
import { EthicsComponent } from './components/pages/ethics/ethics.component';
import { ContactComponent } from './components/pages/contact/contact.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    PageNotFoundComponent,
    MeetTheCreatorsComponent,
    PrivacyComponent,
    ShowArchiveComponent,
    EthicsComponent,
    ContactComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'dni' }),
    BrowserAnimationsModule,
    AppRoutingModule,
    NgStringPipesModule,
    HttpClientModule,
    CookieLawModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
