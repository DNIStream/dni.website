import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from 'app/app.component';
import { HomeComponent } from 'app/components/pages/home/home.component';
import { PageNotFoundComponent } from 'app/components/pages/page-not-found/page-not-found.component';
import { CountdownComponent } from './components/shared/countdown/countdown.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    PageNotFoundComponent,
    CountdownComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'dni' }),
    AppRoutingModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
