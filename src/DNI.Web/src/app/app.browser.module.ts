import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';

import { AppModule } from 'app/app.module';
import { AppComponent } from './components/shared/app.component';

@NgModule({
    imports: [
        AppModule,
        BrowserAnimationsModule
    ],
    providers: [
    ],
    bootstrap: [AppComponent]
})
export class AppBrowserModule { }
