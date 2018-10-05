import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';

import { CookieLawModule } from 'angular2-cookie-law';

import { AppModule } from 'app/app.module';
import { AppComponent } from 'app/app.component';

@NgModule({
    imports: [
        AppModule
    ],
    providers: [
    ],
    bootstrap: [AppComponent]
})
export class AppBrowserModule { }
