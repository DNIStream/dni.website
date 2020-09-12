import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from './components/pages/home/home.component';
import { CommunityGuidelinesComponent } from './components/pages/community-guidelines/community-guidelines.component';
import { FaqComponent } from './components/pages/faq/faq.component';
import { PrivacyComponent } from './components/pages/privacy/privacy.component';
import { ShowArchiveComponent } from './components/pages/show-archive/show-archive.component';
import { ShowDetailComponent } from './components/pages/show-detail/show-detail.component';
import { ContactComponent } from './components/pages/contact/contact.component';
import { EthicsComponent } from './components/pages/ethics/ethics.component';
import { PageNotFoundComponent } from './components/pages/page-not-found/page-not-found.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'community-guidelines', component: CommunityGuidelinesComponent },
  { path: 'faq', component: FaqComponent },
  { path: 'privacy', component: PrivacyComponent },
  { path: 'show-archive', component: ShowArchiveComponent },
  { path: 'show-archive/:keyword', component: ShowArchiveComponent },
  { path: 'show/:slug', component: ShowDetailComponent },
  { path: 'contact', component: ContactComponent },
  { path: 'ethics', component: EthicsComponent },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    scrollPositionRestoration: 'enabled',
    initialNavigation: 'enabled'
  })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
