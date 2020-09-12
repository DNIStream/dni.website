import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';

import { AppComponent } from './components/shared/app.component';
import { AppModule } from './app.module';

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
