import { NgModule } from '@angular/core';
import { ServerModule } from '@angular/platform-server';
import { ModuleMapLoaderModule } from '@nguniversal/module-map-ngfactory-loader';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { AppModule } from 'app/app.module';
import { AppComponent } from 'app/app.component';

@NgModule({
    imports: [
        ServerModule,
        ModuleMapLoaderModule,
        NoopAnimationsModule,
        AppModule
    ],
    providers: [
        // Add universal-only providers here
    ],
    bootstrap: [AppComponent],
})
export class AppServerModule { }
