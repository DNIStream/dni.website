import { NgModule, Optional, SkipSelf } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CookieLawComponent } from './angular2-cookie-law.component';
import { CookieLawContainerComponent } from './angular2-cookie-law-container.component';

@NgModule({
  imports: [
    CommonModule,
  ],
  declarations: [
    CookieLawComponent,
    CookieLawContainerComponent,
  ],
  exports: [
    CookieLawContainerComponent,
  ],
})
export class CookieLawModule {
  constructor (@Optional() @SkipSelf() parentModule: CookieLawModule) {
    if (parentModule) {
      throw new Error(
        'CookieLawModule is already loaded. Import it in the AppModule only');
    }
  }
}
