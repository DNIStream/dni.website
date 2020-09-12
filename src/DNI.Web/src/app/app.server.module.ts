import { NgModule } from '@angular/core';
import { ServerModule } from '@angular/platform-server';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { AppModule } from './app.module';
import { AppComponent } from './components/shared/app.component';

@NgModule({
    imports: [
        ServerModule,
        NoopAnimationsModule,
        AppModule
    ],
    bootstrap: [AppComponent]
})
export class AppServerModule { }
