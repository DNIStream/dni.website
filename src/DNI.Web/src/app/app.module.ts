import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from 'app/app.component';
import { HomeComponent } from 'app/components/pages/home/home.component';
import { PageNotFoundComponent } from 'app/components/pages/page-not-found/page-not-found.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    PageNotFoundComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'dni' }),
    AppRoutingModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
